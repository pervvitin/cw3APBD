
using System;

// Interfejs do wysyłania notyfikacji o zagrożeniach
public interface IHazardNotifier
{
    void NotifyDanger(string containerNumber);
}

// Wyjątek rzucany w przypadku przekroczenia maksymalnej ładowności kontenera
public class OverfillException : Exception
{
    public OverfillException(string message) : base(message) { }
}

// Klasa bazowa dla wszystkich kontenerów
public abstract class Container
{
    public string ContainerNumber { get; private set; }
    public double CargoWeight { get; protected set; }
    public double Height { get; private set; }
    public double OwnWeight { get; private set; }
    public double Depth { get; private set; }
    public double MaxPayload { get; private set; }

    public Container(double cargoWeight, double height, double ownWeight, double depth)
    {
        ContainerNumber = GenerateContainerNumber();
        CargoWeight = cargoWeight;
        Height = height;
        OwnWeight = ownWeight;
        Depth = depth;
        MaxPayload = CalculateMaxPayload();
    }

    private string GenerateContainerNumber()
    {
        // Implementacja generowania numeru seryjnego
        return "KON-" + GetType().Name[0] + "-" + new Random().Next(1, 1000);
    }

    public virtual void LoadCargo(double cargoWeight)
    {
        if (cargoWeight > MaxPayload)
            throw new OverfillException("Cargo weight exceeds maximum payload of the container.");
        CargoWeight += cargoWeight;
    }

    public virtual void EmptyCargo()
    {
        CargoWeight = 0;
    }

    private double CalculateMaxPayload()
    {
        // Implementacja obliczenia maksymalnej ładowności kontenera
        return 1000; // Przykładowa implementacja - do zastąpienia
    }

    public override string ToString()
    {
        return $"Container number: {ContainerNumber}, Cargo weight: {CargoWeight} kg";
    }
}

// Kontener na płyny
public class LiquidContainer : Container, IHazardNotifier
{
    public LiquidContainer(double cargoWeight, double height, double ownWeight, double depth) 
        : base(cargoWeight, height, ownWeight, depth)
    {
    }

    public override void LoadCargo(double cargoWeight)
    {
        if (CargoWeight + cargoWeight > MaxPayload)
            throw new OverfillException("Cargo weight exceeds maximum payload of the container.");
        CargoWeight += cargoWeight;
    }

    public void NotifyDanger(string containerNumber)
    {
        // Implementacja notyfikacji o zagrożeniach
        Console.WriteLine($"Danger notification for container number: {containerNumber}");
    }
}

// Kontener na gaz
public class GasContainer : Container, IHazardNotifier
{
    public double Pressure { get; private set; }

    public GasContainer(double cargoWeight, double height, double ownWeight, double depth, double pressure) 
        : base(cargoWeight, height, ownWeight, depth)
    {
        Pressure = pressure;
    }

    public override void LoadCargo(double cargoWeight)
    {
        if (CargoWeight + cargoWeight > MaxPayload)
            throw new OverfillException("Cargo weight exceeds maximum payload of the container.");
        CargoWeight += cargoWeight;
    }

    public void NotifyDanger(string containerNumber)
    {
        // Implementacja notyfikacji o zagrożeniach
        Console.WriteLine($"Danger notification for container number: {containerNumber}");
    }
}

// Kontener chłodniczy
public class RefrigeratedContainer : Container
{
    public string ProductType { get; private set; }
    public double Temperature { get; private set; }

    public RefrigeratedContainer(double cargoWeight, double height, double ownWeight, double depth, string productType, double temperature) 
        : base(cargoWeight, height, ownWeight, depth)
    {
        ProductType = productType;
        Temperature = temperature;
    }

    public override void LoadCargo(double cargoWeight)
    {
        if (CargoWeight + cargoWeight > MaxPayload)
            throw new OverfillException("Cargo weight exceeds maximum payload of the container.");
        CargoWeight += cargoWeight;
    }
}

// Klasa reprezentująca statek kontenerowiec
public class ContainerShip
{
    public List<Container> Containers { get; private set; }
    public double MaxSpeed { get; private set; }
    public int MaxContainers { get; private set; }
    public double MaxWeight { get; private set; }

    public ContainerShip(double maxSpeed, int maxContainers, double maxWeight)
    {
        MaxSpeed = maxSpeed;
        MaxContainers = maxContainers;
        MaxWeight = maxWeight;
        Containers = new List<Container>();
    }

    public void LoadContainer(Container container)
    {
        // Implementacja ładowania kontenera na statek
        Containers.Add(container);
    }

    public void RemoveContainer(string containerNumber)
    {
        // Implementacja usuwania kontenera ze statku
        Containers.RemoveAll(c => c.ContainerNumber == containerNumber);
    }

    public void ReplaceContainer(string containerNumber, Container newContainer)
    {
        // Implementacja zastępowania kontenera na statku nowym kontenerem
        var index = Containers.FindIndex(c => c.ContainerNumber == containerNumber);
        if (index != -1)
            Containers[index] = newContainer;
    }

    public override string ToString()
    {
        return $"Container ship: Max speed: {MaxSpeed} knots, Max containers: {MaxContainers}, Max weight: {MaxWeight} tons";
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Tworzenie statku kontenerowego
        ContainerShip ship = new ContainerShip(25.5, 100, 50000);

        // Tworzenie kontenerów różnych typów
        LiquidContainer liquidContainer = new LiquidContainer(500, 200, 100, 150);
        GasContainer gasContainer = new GasContainer(3000, 200, 800, 180, 10);
        RefrigeratedContainer refrigeratedContainer = new RefrigeratedContainer(7000, 300, 1200, 220, "Milk", 4);

        // Ładowanie kontenerów na statek
        ship.LoadContainer(liquidContainer);
        ship.LoadContainer(gasContainer);
        ship.LoadContainer(refrigeratedContainer);

        // Wyświetlenie informacji o statku i jego ładunku
        Console.WriteLine(ship.ToString());
        Console.WriteLine("Containers on the ship:");
        foreach (var container in ship.Containers)
        {
            Console.WriteLine(container.ToString());
        }

        // Usunięcie kontenera ze statku
        ship.RemoveContainer(liquidContainer.ContainerNumber);
        Console.WriteLine($"Liquid container {liquidContainer.ContainerNumber} removed from the ship.");

        // Zastąpienie kontenera na statku nowym kontenerem
        LiquidContainer newLiquidContainer = new LiquidContainer(4500, 260, 950, 190);
        ship.ReplaceContainer(gasContainer.ContainerNumber, newLiquidContainer);
        Console.WriteLine($"Gas container {gasContainer.ContainerNumber} replaced with new liquid container {newLiquidContainer.ContainerNumber}.");

        // Wyświetlenie aktualnej listy kontenerów na statku
        Console.WriteLine("Containers on the ship after changes:");
        foreach (var container in ship.Containers)
        {
            Console.WriteLine(container.ToString());
        }
    }
}