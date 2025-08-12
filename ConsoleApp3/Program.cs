using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

// =========================
// QUESTION 1 - Finance App
// =========================

public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[BankTransfer] Processing {transaction.Category}: amount = {transaction.Amount:C}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[MobileMoney] Processing {transaction.Category}: amount = {transaction.Amount:C}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[CryptoWallet] Processing {transaction.Category}: amount = {transaction.Amount:C}");
    }
}

public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        // Default behavior: deduct the amount
        Balance -= transaction.Amount;
        Console.WriteLine($"Account {AccountNumber}: Applied {transaction.Amount:C}. New balance: {Balance:C}");
    }
}

public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
            return;
        }

        Balance -= transaction.Amount;
        Console.WriteLine($"SavingsAccount {AccountNumber}: Deducted {transaction.Amount:C}. Updated balance: {Balance:C}");
    }
}

public class FinanceApp
{
    private readonly List<Transaction> _transactions = new();

    public void Run()
    {
        Console.WriteLine("\n--- FinanceApp Start ---");
        var account = new SavingsAccount("SA-1001", 1000m);

        var t1 = new Transaction(1, DateTime.Now, 120.50m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 250.00m, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 900.00m, "Entertainment");

        var p1 = new MobileMoneyProcessor();
        var p2 = new BankTransferProcessor();
        var p3 = new CryptoWalletProcessor();

        p1.Process(t1);
        p2.Process(t2);
        p3.Process(t3);

        account.ApplyTransaction(t1);
        account.ApplyTransaction(t2);
        account.ApplyTransaction(t3); // might show "Insufficient funds"

        _transactions.AddRange(new[] { t1, t2, t3 });
        Console.WriteLine("--- FinanceApp End ---\n");
    }
}

// =========================
// QUESTION 2 - Health System
// =========================

public class Repository<T>
{
    private readonly List<T> items = new();

    public void Add(T item) => items.Add(item);

    public List<T> GetAll() => new List<T>(items);

    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);

    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item == null) return false;
        return items.Remove(item);
    }
}

public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }
}

public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }
}

public class HealthSystemApp
{
    private readonly Repository<Patient> _patientRepo = new();
    private readonly Repository<Prescription> _prescriptionRepo = new();
    private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new();

    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "Alice Smith", 30, "F"));
        _patientRepo.Add(new Patient(2, "Bob Johnson", 45, "M"));
        _patientRepo.Add(new Patient(3, "Carla Gomez", 28, "F"));

        _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Vitamin D", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Metformin", DateTime.Now.AddDays(-20)));
        _prescriptionRepo.Add(new Prescription(4, 3, "Lisinopril", DateTime.Now.AddDays(-2)));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();
        foreach (var p in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(p.PatientId))
                _prescriptionMap[p.PatientId] = new List<Prescription>();

            _prescriptionMap[p.PatientId].Add(p);
        }
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("\n--- Patients List ---");
        foreach (var p in _patientRepo.GetAll())
        {
            Console.WriteLine($"ID: {p.Id}, Name: {p.Name}, Age: {p.Age}, Gender: {p.Gender}");
        }
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        return _prescriptionMap.TryGetValue(patientId, out var list) ? new List<Prescription>(list) : new List<Prescription>();
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        Console.WriteLine($"\nPrescriptions for patient ID {patientId}:");
        var list = GetPrescriptionsByPatientId(patientId);
        if (!list.Any())
        {
            Console.WriteLine("None found.");
            return;
        }

        foreach (var p in list)
        {
            Console.WriteLine($"Prescription ID: {p.Id}, Medication: {p.MedicationName}, Date: {p.DateIssued:d}");
        }
    }

    public void Run()
    {
        Console.WriteLine("\n--- HealthSystemApp Start ---");
        SeedData();
        BuildPrescriptionMap();
        PrintAllPatients();
        PrintPrescriptionsForPatient(1);
        Console.WriteLine("--- HealthSystemApp End ---\n");
    }
}
// =========================
// QUESTION 3 - Warehouse
// =========================

public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths;
    }
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id; Name = name; Quantity = quantity; ExpiryDate = expiryDate;
    }
}

// Custom exceptions
public class DuplicateItemException : Exception { public DuplicateItemException(string message) : base(message) { } }
public class ItemNotFoundException : Exception { public ItemNotFoundException(string message) : base(message) { } }
public class InvalidQuantityException : Exception { public InvalidQuantityException(string message) : base(message) { } }

public class InventoryRepository<T> where T : IInventoryItem
{
    private readonly Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out var item))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
            throw new ItemNotFoundException($"Item with ID {id} not found for removal.");
    }

    public List<T> GetAllItems() => _items.Values.ToList();

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException("Quantity cannot be negative.");
        var item = GetItemById(id); // will throw if missing
        item.Quantity = newQuantity;
    }
}

public class WareHouseManager
{
    private readonly InventoryRepository<ElectronicItem> _electronics = new();
    private readonly InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        // Electronics
        _electronics.AddItem(new ElectronicItem(1, "Smartphone", 10, "BrandA", 24));
        _electronics.AddItem(new ElectronicItem(2, "Laptop", 5, "BrandB", 12));

        // Groceries
        _groceries.AddItem(new GroceryItem(101, "Rice (5kg)", 20, DateTime.Now.AddMonths(12)));
        _groceries.AddItem(new GroceryItem(102, "Milk (1L)", 30, DateTime.Now.AddDays(10)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        Console.WriteLine($"\n--- Items of type {typeof(T).Name} ---");
        foreach (var item in repo.GetAllItems())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
        }
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Increased stock for ID {id}. New quantity: {repo.GetItemById(id).Quantity}");
        }
        catch (Exception ex) when (ex is ItemNotFoundException || ex is InvalidQuantityException)
        {
            Console.WriteLine($"Error increasing stock: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Removed item ID {id}.");
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
        }
    }

    public void Run()
    {
        Console.WriteLine("\n--- WareHouseManager Start ---");
        SeedData();
        PrintAllItems(_groceries);
        PrintAllItems(_electronics);

        // Try to add duplicate
        try
        {
            _electronics.AddItem(new ElectronicItem(1, "Tablet", 3, "BrandC", 12));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"Caught duplicate add: {ex.Message}");
        }

        // Remove non-existent
        try
        {
            _groceries.RemoveItem(999);
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"Caught remove error: {ex.Message}");
        }

        // Update invalid quantity
        try
        {
            _electronics.UpdateQuantity(2, -5);
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"Caught invalid quantity: {ex.Message}");
        }

        Console.WriteLine("--- WareHouseManager End ---\n");
    }
}

// =========================
// QUESTION 4 - Student Grades from File
// =========================

public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id; FullName = fullName; Score = score;
    }

    public string GetGrade()
    {
        return Score switch
        {
            >= 80 and <= 100 => "A",
            >= 70 and <= 79 => "B",
            >= 60 and <= 69 => "C",
            >= 50 and <= 59 => "D",
            _ => "F"
        };
    }
}

public class InvalidScoreFormatException : Exception { public InvalidScoreFormatException(string msg) : base(msg) { } }
public class MissingFieldException : Exception { public MissingFieldException(string msg) : base(msg) { } }

public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();

        using var reader = new StreamReader(inputFilePath);
        string? line;
        int lineNo = 0;
        while ((line = reader.ReadLine()) != null)
        {
            lineNo++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');
            if (parts.Length < 3)
                throw new MissingFieldException($"Line {lineNo}: Missing fields. Expected 3 values.");

            var idPart = parts[0].Trim();
            var namePart = parts[1].Trim();
            var scorePart = parts[2].Trim();

            if (!int.TryParse(idPart, out var id))
                throw new InvalidScoreFormatException($"Line {lineNo}: ID '{idPart}' is not a valid integer.");

            if (!int.TryParse(scorePart, out var score))
                throw new InvalidScoreFormatException($"Line {lineNo}: Score '{scorePart}' is not a valid integer.");

            students.Add(new Student(id, namePart, score));
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using var writer = new StreamWriter(outputFilePath);
        foreach (var s in students)
        {
            writer.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
        }
    }

    public void RunDemo(string inputPath, string outputPath)
    {
        Console.WriteLine("\n--- StudentResultProcessor Start ---");
        try
        {
            var students = ReadStudentsFromFile(inputPath);
            WriteReportToFile(students, outputPath);
            Console.WriteLine($"Report written to {outputPath}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"File not found: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Missing field error: {ex.Message}");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Invalid score format: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
        Console.WriteLine("--- StudentResultProcessor End ---\n");
    }
}


