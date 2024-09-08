# BTProx

This project is inspired on the project <https://github.com/ewenchou/bluetooth-proximity> that became unmaintained.

This is a simple first version.

## Logical steps

- Install Bluetooth dependencies: You can use `BlueZ`, which is the official Linux Bluetooth protocol stack. Use
  `hcitool` to scan Bluetooth devices.
- Locking and Unlocking the PC: For locking the screen, you can use `xdg-screensaver lock` or `loginctl lock-session`,
  and for unlocking, you can simulate a keypress or mouse event.

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

## Steps

1. Set your phone’s Bluetooth MAC address in the `phoneBluetoothAddress` variable.

1. Lock screen command: Uses `xdg-screensaver` lock to lock the screen.

1. Unlock screen logic: Currently uses `xdotool` to simulate mouse activity, which will wake the screen. Adjust as
   needed based on your environment.

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
- This can be extended further for more security or custom interactions, like requiring authentication or a specific
  Bluetooth signal strength threshold for proximity detection.
