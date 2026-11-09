// prieks visiem projektiem tika izmantots c# vadlinijas un dokumentacija no https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/
// 

using HabitTracker.ConsoleApp;
using HabitTracker.Domain;
using System;
using System.Linq;

var habits = TestDataGenerator.GenerateHabits();   // izsauc testa datus funkciju, kura ir statiska


Console.WriteLine("Ieradumu Parvaldnieks");
Console.WriteLine($"Kopejais testa ieradumu skaits: {habits.Count}\n");  


var users = habits.Select(h => h.UserID).Distinct(); // iegust neduplicetus lietotajus
Console.WriteLine($"Lietotaji sistema: {string.Join(", ", users)}\n");


Console.WriteLine("Visi lietotaji:");
foreach (var habit in habits) // iziet cauri viesiem ieradumiem
{
    string status = habit.IsArchived ? "Arhivets" : "Aktivs";

    bool isCompletedToday = habit.IsCompletedToday(); // salidzina sodienu un izpildes datumu
    string completedText = isCompletedToday ? "ja" : "ne";  
    //izprinte ieraksta datus
    Console.WriteLine($"Lietotajs: {habit.UserID}");
    Console.WriteLine($"Ieradums: '{habit.Name}'");
    Console.WriteLine($"Apraksts: {habit.Description}");
    Console.WriteLine($"Statuss: {status}");
    Console.WriteLine($"Sodien izpildits: {completedText}");
    Console.WriteLine();
    
}
// datu parbaude nemta no gemini, pasam nesanaca veiksmigi izmainit datus

Console.WriteLine("Datu pārbaudes tests");
try
{
    Console.WriteLine($"Mēģinam ieradumam '{habits[0].Name}' iestatīt tukšu nosaukumu...");

    // bus kluda jo nosaukums tukss
    habits[0].Name = "";

    Console.WriteLine("Ja redzi šo, kļūda nenostrādāja!");
}
catch (Exception ex)
{
    // parada kludu sarkana krasa
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"KLUDA: {ex.Message}");
    Console.ResetColor();
}