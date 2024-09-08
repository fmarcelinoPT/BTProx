// See https://aka.ms/new-console-template for more information

// Install Bluetooth Utilities
// sudo apt-get install bluez xdotool

using System.Diagnostics;

var phoneBluetoothAddress = "A4:75:B9:5F:B0:41"; // Replace with your phone's Bluetooth MAC address


async Task<bool> IsPhoneNearby()
{
    try
    {
        // Run the hcitool to scan for the phone's Bluetooth MAC address
        Process process = new Process();
        process.StartInfo.FileName = "hcitool";
        process.StartInfo.Arguments = $"name {phoneBluetoothAddress}";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        // If the output contains the phone's Bluetooth name, it is nearby
        return !string.IsNullOrEmpty(output);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error detecting Bluetooth device: {ex.Message}");
        return false;
    }
}

void LockScreen()
{
    // Execute the lock screen command
    ExecuteCommand("xdg-screensaver lock");
}

void UnlockScreen()
{
    // Simulate an unlock (could be replaced with actual unlock logic if needed)
    ExecuteCommand("xdotool mousemove 0 0"); // Moves the mouse to simulate activity
}

void ExecuteCommand(string command)
{
    try
    {
        Process process = new Process();
        process.StartInfo.FileName = "bash";
        process.StartInfo.Arguments = $"-c \"{command}\"";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine($"Command error: {error}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error executing command: {ex.Message}");
    }
}

bool isPhoneNearby = false;

while (true)
{
    bool phoneDetected = await IsPhoneNearby();

    if (phoneDetected && !isPhoneNearby)
    {
        // Unlock the screen if the phone is detected
        Console.WriteLine("Phone is nearby. Unlocking the screen.");
        UnlockScreen();
        isPhoneNearby = true;
    }
    else if (!phoneDetected && isPhoneNearby)
    {
        // Lock the screen if the phone is not detected
        Console.WriteLine("Phone is not nearby. Locking the screen.");
        LockScreen();
        isPhoneNearby = false;
    }

    await Task.Delay(5000); // Check every 5 seconds
}