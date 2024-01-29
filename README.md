# MIOS.net
First stab at a multiplatform GUI controller for OpenSimulator. The original MIOS was a script written in PHP and is well past it's sell by date now. It was basically just a .ini file manager for OpenSimulator that would take all of the complexity out of creating well configured instances. This is a complete rewrite in .net 6.0 and will be a lot more than just a .ini file manager (hopefully).

Requirements:
* .net 6.0 or above
* Multiplatform (Windows, Linux, Mac)
* Run as a desktop app or via a web browser remotely
* Support for configuring multiple grids
* Support for configuring multiple regions
* Support for configuring multiple instances hosting one or more regions
* User/Avatar management etc
* Extra plugins for specific functionality (e.g. Hypergrid, Groups, etc)

This will be quite a lot like Dreamgrid. but with a few differences - see the first 3 items above.

The GUI interface for this is actually just a web interface that can be run in a browser. The backend is a .net 6.0 app that can be run as a desktop app or as a web app. The desktop app version is in fact just wrapping the web app in a platform native browser window which points to the web app on localhost. I chose PhotinoNET rather and Microsoft's WebView2 because it is a lot smaller and simpler to use. It also has a lot less dependencies and is many times smaller in size.

Soo much work to do on this. I'm just getting started. I'm not even sure if I'll be able to get it to work. But I'm going to give it a go. :)