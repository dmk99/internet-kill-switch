# internet-kill-switch
The idea came about for this project because some VPN services do not have a built-in killswitch. It's kind of pointless for a VPN to automatically default to the unencrypted netwrok connection when the connection times out or closes for some reason.

This project is designed for Windows 7+ and uses .net 4.0.

TODO: <br/>
1. Diagnose and fix UI application crash after having been run for a few hours (most likely timeout with service).<br/>
2. Refactor code where appropriate.<br/>
3. Implement Stop/Start service from the UI.<br/>
4. Include Icon for the UI.<br/>
5. Return the current IP address.<br/>
6. Have a notify popup informing the user when something has changed in their network.<br/>
7. WiX installer.<br/>
