using System;
using System.Runtime.InteropServices;

public static class LibEmbed
{
    private static IntPtr ffi;

    static LibEmbed()
    {
        string path;
        string extension;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            path = "win/";
            extension = ".dll";
        }
        else
        {
            path = "lin/";
            extension = ".so";
        }

        string name;

        if (RuntimeInformation.OSArchitecture == Architecture.X64)
        {
            name = "lib-embed-x64";
        }
        else
        {
            name = "lib-embed-x86";
        }

        string fullPath = path + name + extension;
        ffi = NativeLibrary.Load(fullPath);

        if (ffi == IntPtr.Zero)
        {
            throw new Exception($"Failed to load library: {fullPath}");
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr EmbedObterValorDelegate(IntPtr json, IntPtr key);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr EmbedConfigurarDelegate(IntPtr input);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr EmbedIniciarDelegate(IntPtr input);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr EmbedProcessarDelegate(IntPtr input);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr EmbedFinalizarDelegate(IntPtr input);

    private static readonly EmbedObterValorDelegate EmbedObterValor;
    private static readonly EmbedConfigurarDelegate EmbedConfigurar;
    private static readonly EmbedIniciarDelegate EmbedIniciar;
    private static readonly EmbedProcessarDelegate EmbedProcessar;
    private static readonly EmbedFinalizarDelegate EmbedFinalizar;

    static LibEmbed()
    {
        IntPtr pAddressOfFunctionToCall = NativeLibrary.GetExport(ffi, "embed_obter_valor");
        EmbedObterValor = Marshal.GetDelegateForFunctionPointer<EmbedObterValorDelegate>(pAddressOfFunctionToCall);

        pAddressOfFunctionToCall = NativeLibrary.GetExport(ffi, "embed_configurar");
        EmbedConfigurar = Marshal.GetDelegateForFunctionPointer<EmbedConfigurarDelegate>(pAddressOfFunctionToCall);

        pAddressOfFunctionToCall = NativeLibrary.GetExport(ffi, "embed_iniciar");
        EmbedIniciar = Marshal.GetDelegateForFunctionPointer<EmbedIniciarDelegate>(pAddressOfFunctionToCall);

        pAddressOfFunctionToCall = NativeLibrary.GetExport(ffi, "embed_processar");
        EmbedProcessar = Marshal.GetDelegateForFunctionPointer<EmbedProcessarDelegate>(pAddressOfFunctionToCall);

        pAddressOfFunctionToCall = NativeLibrary.GetExport(ffi, "embed_finalizar");
        EmbedFinalizar = Marshal.GetDelegateForFunctionPointer<EmbedFinalizarDelegate>(pAddressOfFunctionToCall);
    }

    public static string ObterValor(string json, string key)
    {
        IntPtr jsonPtr = Marshal.StringToHGlobalAnsi(json);
        IntPtr keyPtr = Marshal.StringToHGlobalAnsi(key);

        IntPtr resultPtr = EmbedObterValor(jsonPtr, keyPtr);

        string result = Marshal.PtrToStringAnsi(resultPtr);

        Marshal.FreeHGlobal(jsonPtr);
        Marshal.FreeHGlobal(keyPtr);

        return result;
    }

    public static string Configurar(string input)
    {
        IntPtr inputPtr = Marshal.StringToHGlobalAnsi(input);

        IntPtr resultPtr = EmbedConfigurar(inputPtr);

        string result = Marshal.PtrToStringAnsi(resultPtr);

        Marshal.FreeHGlobal(inputPtr);

        return result;
    }

    public static string Iniciar(string input)
    {
        IntPtr inputPtr = Marshal.StringToHGlobalAnsi(input);

        IntPtr resultPtr = EmbedIniciar(inputPtr);

        string result = Marshal.PtrToStringAnsi(resultPtr);

        Marshal.FreeHGlobal(inputPtr);

        return result;
    }

    public static string Processar(string input)
    {
        IntPtr inputPtr = Marshal.StringToHGlobalAnsi(input);

        IntPtr resultPtr = EmbedProcessar(inputPtr);

        string result = Marshal.PtrToStringAnsi(resultPtr);

        Marshal.FreeHGlobal(inputPtr);

        return result;
    }

    public static string Finalizar(string input)
    {
        IntPtr inputPtr = Marshal.StringToHGlobalAnsi(input);

        IntPtr resultPtr = EmbedFinalizar(inputPtr);

        string result = Marshal.PtrToStringAnsi(resultPtr);

        Marshal.FreeHGlobal(inputPtr);

        return result;
    }
}
