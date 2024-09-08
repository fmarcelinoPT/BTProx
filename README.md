# BluetoothLockUnlock

To create a C# program that locks and unlocks your Linux PC based on the proximity of your phone's Bluetooth, you'll need to use Bluetooth APIs to detect the phone's presence and interact with Linux system commands for locking and unlocking.

## Logical steps

- Install Bluetooth dependencies: You can use `BlueZ`, which is the official Linux Bluetooth protocol stack. Use `hcitool` to scan Bluetooth devices.
- Locking and Unlocking the PC: For locking the screen, you can use `xdg-screensaver lock` or `loginctl lock-session`, and for unlocking, you can simulate a keypress or mouse event.

You can write a C# program to run on Linux. Here’s a basic outline:

## Prerequisites

1. Install .NET Core SDK:

   - For Ubuntu, you can install .NET Core by following these steps:

   ```bash
   sudo apt-get update
   sudo apt-get install -y dotnet-sdk-7.0  # Or any supported .NET version
   ```

1. Install Bluetooth Utilities:

   - You'll need BlueZ (for Bluetooth scanning) and possibly xdotool for screen locking/unlocking.

   ```bash
   sudo apt-get install bluez xdotool
   ```

## C# Code for Bluetooth Proximity Lock/Unlock

```csharp
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

class BluetoothProximityLock
{
private static string phoneBluetoothAddress = "XX:XX:XX:XX:XX:XX"; // Replace with your phone's Bluetooth MAC address

    static async Task Main(string[] args)
    {
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
    }

    private static async Task<bool> IsPhoneNearby()
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

    private static void LockScreen()
    {
        // Execute the lock screen command
        ExecuteCommand("xdg-screensaver lock");
    }

    private static void UnlockScreen()
    {
        // Simulate an unlock (could be replaced with actual unlock logic if needed)
        ExecuteCommand("xdotool mousemove 0 0"); // Moves the mouse to simulate activity
    }

    private static void ExecuteCommand(string command)
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
}
```

## Steps

1. Set your phone’s Bluetooth MAC address in the `phoneBluetoothAddress` variable.

1. Lock screen command: Uses `xdg-screensaver` lock to lock the screen.

1. Unlock screen logic: Currently uses `xdotool` to simulate mouse activity, which will wake the screen. Adjust as needed based on your environment.

1. Bluetooth Scanning: The program uses `hcitool` to check if your phone is nearby based on its Bluetooth MAC address.

## Compiling and Running in .NET Core

1. Create a new .NET project:

   ```bash
   dotnet new console -n BluetoothProximityLock
   cd BluetoothProximityLock
   ```

1. Replace the content of Program.cs with the C# code provided above.

1. Build the project:

   ```bash
   dotnet build
   ```

1. Run the project:

   ```bash
   dotnet run
   ```

## Summary

- This program checks every 5 seconds whether your phone's Bluetooth signal is nearby using `hcitool`.
- If detected, it simulates unlocking the screen. If not detected, it locks the screen.
- This can be extended further for more security or custom interactions, like requiring authentication or a specific Bluetooth signal strength threshold for proximity detection.

## TODO

1. Measure bluetooth distance to lock/unlock
   - <https://linux.die.net/man/1/hcitool>
   - <https://forums.raspberrypi.com/viewtopic.php?t=47466>
