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
