using System.Runtime.InteropServices;

namespace MediaPipeForDOTS {
    
    [StructLayout(LayoutKind.Sequential)]
    public struct GlTextureInfo {
        
        public int glInternalFormat;
        public uint glFormat;
        public uint glType;
        public int downscale;
    }
}