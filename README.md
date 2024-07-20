# Port Knocking Utility

Port Knocking Utility is a C# application that demonstrates port knocking for network services. The application performs a secure login and then allows the user to connect to a server using SSH, SFTP, or RDP after performing port knocking to open the necessary ports.

## Features

- **Secure Login**: Authenticate users with a hashed password using BCrypt.
- **Port Knocking**: Open and close ports on a remote server to prepare for connections.
- **Protocol Options**: Connect to the server using SSH, SFTP, or RDP.
- **User Input**: Accepts user credentials and server information from the command line.

## Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- A Windows environment with SSH, SFTP, and RDP clients installed.
- [knockd](https://github.com/ChrisLPJones/Port-Knocking-Utility) installed on the server.

## Installation

## Installing `knockd` on the Server

1. **Update the package list and install `knockd`:**

    ```bash
    sudo apt update
    sudo apt install knockd
    ```

2. **Configure `knockd`:**

    Edit the `knockd` configuration file, usually located at `/etc/knockd.conf`:

    ```bash
    sudo nano /etc/knockd.conf
    ```

    Add the following configuration to the file:

    ```ini
    [options]
        UseSyslog

    [openSSH]
        sequence    = 7000:udp,8000:udp,9000:udp
        seq_timeout = 5
        command     = /sbin/iptables -C INPUT -p tcp --dport 22 -j ACCEPT || iptables -A INPUT -p tcp --dport 22 -j ACCEPT
        tcpflags    = syn,ack

    [closeSSH]
        sequence    = 9000:udp,8000:udp,7000:udp
        seq_timeout = 5
        command     = /sbin/iptables -D INPUT -p tcp --dport 22 -j ACCEPT
        tcpflags    = syn,ack

    [openRDP]
        sequence    = 4000:udp,5000:udp,6000:udp
        seq_timeout = 5
        command     = /sbin/iptables -C INPUT -p tcp --dport 3389 -j ACCEPT || iptables -A INPUT -p tcp --dport 3389 -j ACCEPT
        tcpflags    = syn,ack

    [closeRDP]
        sequence    = 6000:udp,5000:udp,4000:udp
        seq_timeout = 5
        command     = /sbin/iptables -D INPUT -p tcp --dport 3389 -j ACCEPT
        tcpflags    = syn,ack
    ```

    **Note**: Make sure to replace `/sbin/iptables` with the correct path to the `iptables` executable if it's different on your system.

3. **Start and enable `knockd` service:**

    ```bash
    sudo systemctl start knockd
    sudo systemctl enable knockd
    ```

1. **Clone the repository:**

    ```bash
    git clone https://github.com/ChrisLPJones/PortKnockingUtility.git
    ```

2. **Navigate to the project directory:**

    ```bash
    cd PortKnockingUtility
    ```

3. **Restore the project dependencies:**

    ```bash
    dotnet restore
    ```

4. **Build the project:**

    ```bash
    dotnet build
    ```

5. **Run the application:**

    ```bash
    dotnet run
    ```


## Usage

1. **Run the application.**

2. **Enter your username and password when prompted. The password is validated using a BCrypt hash.**

3. **After a successful login, enter the server address.**

4. **Choose a connection type from the following options:**
    - **1**: Connect with SSH
    - **2**: Connect with SFTP
    - **3**: Connect with RDP
    - **4**: Exit the application

5. **The application will perform port knocking by opening and closing predefined ports, then initiate the selected connection type.**

## Dependencies

- `BCrypt.Net-Next`: A .NET library for BCrypt hashing.

## Notes

- Ensure `knockd` is properly configured and tested.
- Adjust the port numbers in the `knockd` configuration to match your specific requirements if different from the defaults.
- For `knockd` to work correctly, make sure the service is running and enabled to start on boot.
- Review and adapt the configuration to match your network security needs.
- **Change the BCrypt hash in the source code to match your own secure password.** The default hash is for demonstration purposes only and should be replaced.
