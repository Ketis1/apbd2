// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");

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
    //TO DO: dodac statek i poszczegolne rodzaje konetenerow
}