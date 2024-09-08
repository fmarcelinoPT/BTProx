// See https://aka.ms/new-console-template for more information

// Install Bluetooth Utilities
// sudo apt-get install bluez xdotool

using System.Diagnostics;

var phoneBluetoothAddress = "A4:75:B9:5F:B0:41"; // Replace with your phone's Bluetooth MAC address
var rssiThreshold = -60; // Approximate RSSI for 1 meter distance


async Task<int?> GetPhoneRssi()
{
    try
    {
        // Run hcitool to get the RSSI of the phone's Bluetooth MAC address
        var process = new Process();
        process.StartInfo.FileName = "hcitool";
        process.StartInfo.Arguments = $"rssi {phoneBluetoothAddress}";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        // Example output: "RSSI return value: -55"
        if (output.Contains("RSSI return value"))
        {
            var parts = output.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[1].Trim(), out var rssi)) return rssi;
        }

        return null; // If no RSSI found
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error detecting Bluetooth device: {ex.Message}");
        return null;
    }
}

void LockScreen()
{
    // Execute the lock screen command
    ExecuteCommand("xdg-screensaver lock");
}

void UnlockScreen()
{
    // Simulate an unlock by moving the mouse
    ExecuteCommand("xdotool mousemove 0 0");
}

void ExecuteCommand(string command)
{
    try
    {
        var process = new Process();
        process.StartInfo.FileName = "bash";
        process.StartInfo.Arguments = $"-c \"{command}\"";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.Start();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (!string.IsNullOrEmpty(error)) Console.WriteLine($"Command error: {error}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error executing command: {ex.Message}");
    }
}

// #######################

var isPhoneNearby = false;

while (true)
{
    var phoneRssi = await GetPhoneRssi();

    if (phoneRssi.HasValue && phoneRssi > rssiThreshold && !isPhoneNearby)
    {
        // Unlock the screen if the phone is within 1 meter
        Console.WriteLine($"Phone is nearby (RSSI: {phoneRssi}). Unlocking the screen.");
        UnlockScreen();
        isPhoneNearby = true;
    }
    else if ((!phoneRssi.HasValue || phoneRssi <= rssiThreshold) && isPhoneNearby)
    {
        // Lock the screen if the phone is farther than 1 meter or not detected
        Console.WriteLine($"Phone is not nearby (RSSI: {phoneRssi}). Locking the screen.");
        LockScreen();
        isPhoneNearby = false;
    }

    await Task.Delay(5000); // Check every 5 seconds
}