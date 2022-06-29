sing System.Diagnostics;
	using System.Runtime.InteropServices;
	
	namespace TXTextControl.Diagnostics {
	
	    /// <summary>
	    /// Singleton factory class to return and manage ServerTextControl instances.
	    /// </summary>
	    public sealed class ServerTextControlFactory {
	
	        // private fields
	        private static readonly ServerTextControlFactory _instance =
	            new ServerTextControlFactory();
	        private List<ServerTextControl> _activeControls;
	
	        [DllImport("User32")]
	        extern public static int GetGuiResources(IntPtr hProcess, int uiFlags);
	
	        private enum ResourceType {
	            Gdi = 0,
	            User = 1
	        }
	
	        static ServerTextControlFactory() {
	
	        }
	
	        private ServerTextControlFactory() {
	            _activeControls = new List<ServerTextControl>();
	        }
	
	        /// <summary>
	        /// Returns the singleton instance.
	        /// </summary>
	        public static ServerTextControlFactory Instance {
	            get {
	                return _instance;
	            }
	        }
	
	        /// <summary>
	        /// Returns the number of active ServerTextControls that are not disposed.
	        /// </summary>
	        public int Count {
	            get {
	                return _activeControls.Count;
	            }
	        }
	
	        /// <summary>
	        /// Returns the total number of USER handles in current process.
	        /// </summary>
	        public int UserHandles {
	            get {
	                var processHandle = Process.GetCurrentProcess().Handle;
	                return GetGuiResources(processHandle, (int)ResourceType.User);
	            }
	        }
	
	        /// <summary>
	        /// Returns the total number of GDI handles in current process.
	        /// </summary>
	        public int GdiHandles {
	            get {
	                var processHandle = Process.GetCurrentProcess().Handle;
	                return GetGuiResources(processHandle, (int)ResourceType.Gdi);
	            }
	        }
	
	        /// <summary>
	        /// Disposes all active ServerTextControl instances.
	        /// </summary>
	        public int DisposeAll() {
	
	            int numControls = _activeControls.Count;
	            int i;
	
	            for (i = 0; i < numControls; i++) {
	                _activeControls[0].Dispose();
	            }
	
	            return i;
	        }
	
	        /// <summary>
	        /// Returns a new ServerTextControl instance and adds it to the list of
	        /// active instances.
	        /// Returns null in case a new ServerTextControl instance could not be created.
	        /// </summary>
	        public ServerTextControl? CreateServerTextControl() {
	
	            // create a new ServerTextControl instance and listen for the disposal
	            ServerTextControl serverTextControl = new ServerTextControl();
	
	            // trying to create - if creation fails, dispose the object and return null
	            if (serverTextControl.Create() == false) {
	                serverTextControl.Dispose();
	                return null;
	            }
	
	            // attached the Disposed event to clean up list of instances
	            serverTextControl.Disposed += ServerTextControl_Disposed;
	
	            // add to list of instances
	            _activeControls.Add(serverTextControl);
	
	            return serverTextControl;
	        }
	
	        private void ServerTextControl_Disposed(object sender, EventArgs e) {
	            // remove from list of instances
	            _activeControls.Remove((ServerTextControl)sender);
	        }
	    }
	}
