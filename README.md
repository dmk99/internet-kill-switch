# internet-kill-switch
The idea came about for this project because some VPN services do not have a built-in killswitch. It's kind of pointless for a VPN to automatically default to the unencrypted netwrok connection when the connection times out or closes for some reason.

This project is designed for Windows 7+ and uses .net 4.0.

TODO:
1. Create taskbar application for interaction with the service.<br/>
2. Allow user to easily add their networks to either the VPN or Local lists.<br/>
3. Save service state (when a user adds/removes a network adapter from the lists save. When started up again reload.)<br/>
4. WiX installer.
