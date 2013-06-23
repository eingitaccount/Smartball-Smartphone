Smartball-Smartphone
====================
(ID 2013)

Initial master commit already contains quite some content, like:
A (temporary?) UI showing the device's acceleration.
Possibility to connect to a host by entering the ip.
Sending messages to the host. Those
  - contain some values for x and z, calculated by the sensors of the device and some magical math [which must be improved]
  - are sent every 60 ms - maybe we need another value
  - doesn't contain information about the camera yet
We also have a help page giving a litte help to the user and alow him to contact the developers by mail.
  
Code isn't strong, your occasional exception is stil around. Not really tested yet. OOP ignored at (too) many places, refactoring will be fun.

By now: Use branches.
