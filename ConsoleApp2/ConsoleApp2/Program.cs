

public interface IContainer
{
    string SerialNumber { get; }
    double Height { get; }
    double Depth { get; }
    double CargoWeight { get; }
    double ContainerWeight { get; }
    double MaxLoad { get; }


    void LoadCargo(double weight);
    void UnloadCargo();
}
public interface IHazardNotifier
{
    void NotifyDanger(string containerSerialNumber);
}


public class Container : IContainer
{
    private static int serialNumberCounter = 1;
    
    public string SerialNumber { get; }
    public double Height { get; }
    public double Depth { get; }
    public double CargoWeight { get; set; }
    public double ContainerWeight { get; }
    public double MaxLoad { get; }


    public Container(double maxCapacity)
    {
        SerialNumber = "KON-C-" + serialNumberCounter++;
        MaxLoad = maxCapacity;
    }
    public void LoadCargo(double weight)
    {
        if (CargoWeight + weight > MaxLoad)
        {
            throw new OverflowException($"Overfilling container {SerialNumber}");
        }

        CargoWeight += weight;
    }

    public void UnloadCargo()
    {
        CargoWeight = 0;
    }
    
    public string GetContainerInfo()
    {
        return $"Serial Number: {SerialNumber}, Height: {Height}, Depth: {Depth}, Cargo Weight: {CargoWeight}, Container Weight: {ContainerWeight}, Max Load: {MaxLoad}";
    }
}


public class LiquidContainer : Container, IHazardNotifier
{
    private bool isHazardous;

    public LiquidContainer(double maxCapacity, bool isHazardous) : base(maxCapacity)
    {
        this.isHazardous = isHazardous;
    }

    public void NotifyDanger(string containerSerialNumber)
    {
        Console.WriteLine($"Dangerous situation in container {containerSerialNumber}");
    }

    public void LoadCargo(double weight)
    {
        if (isHazardous)
        {
            if (CargoWeight + weight > CargoWeight * 0.5)
            {
                NotifyDanger(this.SerialNumber);
            }
        }
        else
        {
            if (CargoWeight + weight > MaxLoad * 0.9)
            {
                NotifyDanger(this.SerialNumber);
            }
        }

        base.LoadCargo(weight);
    }
    public string GetContainerInfo()
    {
        return base.GetContainerInfo() + $", Hazardous: {isHazardous}";
    }
    
}

public class BingChillingContainer : Container
{
    public double RequiredTemperature;
    public string ProductType { get; }
    public double Temperature { get; set; }

    public BingChillingContainer(double maxCapacity, string productType, double initialTemperature) : base(maxCapacity)
    {
        ProductType = productType;
        if (!ProductTemperatures.TryGetValue(productType, out double requiredTemperature))
        {
            throw new ArgumentException($"Product {productType} is not in the temperature table.");
        }
        RequiredTemperature = requiredTemperature;
        Temperature = initialTemperature;
    }
    
    
    
    public static readonly Dictionary<string, double> ProductTemperatures = new Dictionary<string, double>()
    {
        {"Bananas", 4.0}, 
        {"Fish", 6.0},    
        {"Eggs", 2.0},  
        
    };
    public string GetContainerInfo()
    {
        return base.GetContainerInfo() + $", Product Type: {ProductType}, Required Temperature: {RequiredTemperature}, Current Temperature: {Temperature}";
    }
}

public class GasContainer : Container, IHazardNotifier
{
    private double pressure;

    public GasContainer(double maxCapacity, double pressure) : base(maxCapacity)
    {
        this.pressure = pressure;
    }

    public void NotifyDanger(string containerSerialNumber)
    {
        Console.WriteLine($"Dangerous situation in gas container {containerSerialNumber}");
    }

    public void UnloadCargo()
    {
        base.UnloadCargo();
        // Pozostawiamy 5% ładunku wewnątrz kontenera
        CargoWeight = MaxLoad * 0.05;
    }
    public void LoadCargo(double weight)
    {
        if (CargoWeight + weight > MaxLoad)
        {
            NotifyDanger(this.SerialNumber);
        }
        CargoWeight += weight;
    }
    public string GetContainerInfo()
    {
        return base.GetContainerInfo() + $", Pressure: {pressure}";
    }
}



public class ContainerShip
{
    public List<Container> Containers { get; private set; }
    public double MaxSpeedInKnots { get; }
    public int MaxContainerCount { get; }
    public double MaxTotalWeightInTons { get; }

    public ContainerShip(double maxSpeedInKnots, int maxContainerCount, double maxTotalWeightInTons)
    {
        MaxSpeedInKnots = maxSpeedInKnots;
        MaxContainerCount = maxContainerCount;
        MaxTotalWeightInTons = maxTotalWeightInTons;
        Containers = new List<Container>();
    }

    public void LoadContainer(Container container)
    {
        if (Containers.Count >= MaxContainerCount)
        {
            throw new InvalidOperationException("Cannot load more containers. Maximum container count reached.");
        }
        if ( (Containers.Sum(c => c.CargoWeight+c.ContainerWeight) + container.CargoWeight + container.ContainerWeight) *1000 > MaxTotalWeightInTons)
        {
            throw new InvalidOperationException("Cannot load container. Maximum total weight reached.");
        }
        Containers.Add(container);
    }

    public void UnloadContainer(Container container)
    {
        Containers.Remove(container);
    }

    public void ReplaceContainer(string containerSerialNumber, Container newContainer)
    {
        var containerToRemove = Containers.FirstOrDefault(c => c.SerialNumber == containerSerialNumber);
        if (containerToRemove == null)
        {
            throw new ArgumentException("Container with specified serial number not found.");
        }
        Containers.Remove(containerToRemove);
        LoadContainer(newContainer);
    }

    public void MoveContainer(Container container, ContainerShip destinationShip)
    {
        UnloadContainer(container);
        destinationShip.LoadContainer(container);
    }

    public void PrintContainerInfo(Container container)
    {
        Console.WriteLine(container.GetContainerInfo());
    }

    public void PrintShipInfo()
    {
        Console.WriteLine($"Max Speed: {MaxSpeedInKnots} knots");
        Console.WriteLine($"Max Container Count: {MaxContainerCount}");
        Console.WriteLine($"Max Total Weight: {MaxTotalWeightInTons} tons");
        Console.WriteLine($"Number of Loaded Containers: {Containers.Count}");
        foreach (var container in Containers)
        {
            Console.WriteLine(container.GetContainerInfo());
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        try
        {

            LiquidContainer liquidContainer = new LiquidContainer(100, true);
            BingChillingContainer chillingContainer = new BingChillingContainer(200, "Bananas", 5.0);
            GasContainer gasContainer = new GasContainer(150, 2.5);

            ContainerShip containerShip = new ContainerShip(20, 10, 300);
            
            containerShip.LoadContainer(liquidContainer);
            containerShip.LoadContainer(chillingContainer);
            containerShip.LoadContainer(gasContainer);
            
            containerShip.PrintShipInfo();
            
            containerShip.PrintContainerInfo(chillingContainer);

            BingChillingContainer newChillingContainer = new BingChillingContainer(180, "Fish", 4.5);
            containerShip.ReplaceContainer(chillingContainer.SerialNumber, newChillingContainer);

            containerShip.PrintShipInfo();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}