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