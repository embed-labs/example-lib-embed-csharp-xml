using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dotenv.net;

public static class EmbedLibLoader
{
    public static IntPtr ffi;

    static EmbedLibLoader()
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
}

public static class EmbedLib
{
    private delegate IntPtr EmbedObterValorDelegate(IntPtr json, IntPtr key);
    private delegate IntPtr EmbedConfigurarDelegate(IntPtr input);
    private delegate IntPtr EmbedIniciarDelegate(IntPtr input);
    private delegate IntPtr EmbedProcessarDelegate(IntPtr input);
    private delegate IntPtr EmbedFinalizarDelegate(IntPtr input);

    private static readonly EmbedObterValorDelegate EmbedObterValor;
    private static readonly EmbedConfigurarDelegate EmbedConfigurar;
    private static readonly EmbedIniciarDelegate EmbedIniciar;
    private static readonly EmbedProcessarDelegate EmbedProcessar;
    private static readonly EmbedFinalizarDelegate EmbedFinalizar;

    static EmbedLib()
    {
        IntPtr pAddressOfFunctionToCall = NativeLibrary.GetExport(EmbedLibLoader.ffi, "embed_obter_valor");
        EmbedObterValor = Marshal.GetDelegateForFunctionPointer<EmbedObterValorDelegate>(pAddressOfFunctionToCall);

        pAddressOfFunctionToCall = NativeLibrary.GetExport(EmbedLibLoader.ffi, "embed_configurar");
        EmbedConfigurar = Marshal.GetDelegateForFunctionPointer<EmbedConfigurarDelegate>(pAddressOfFunctionToCall);

        pAddressOfFunctionToCall = NativeLibrary.GetExport(EmbedLibLoader.ffi, "embed_iniciar");
        EmbedIniciar = Marshal.GetDelegateForFunctionPointer<EmbedIniciarDelegate>(pAddressOfFunctionToCall);

        pAddressOfFunctionToCall = NativeLibrary.GetExport(EmbedLibLoader.ffi, "embed_processar");
        EmbedProcessar = Marshal.GetDelegateForFunctionPointer<EmbedProcessarDelegate>(pAddressOfFunctionToCall);

        pAddressOfFunctionToCall = NativeLibrary.GetExport(EmbedLibLoader.ffi, "embed_finalizar");
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

public class Program
{
    public const string STATUS_CODE = "resultado.status_code";

    public static List<string> ListarArquivos(string diretorio, string extensao)
    {
        var arquivos = Directory.EnumerateFiles(diretorio, $"*.{extensao}", SearchOption.AllDirectories).ToList();
        return arquivos;
    }

    public static void Configurar()
    {
        DotEnv.Load();

        string PRODUTO = "xml";                 // produto atual xml
        string SUB_PRODUTO = "1";               // produto atual datalake
        string ACCESS_KEY = Environment.GetEnvironmentVariable("ACCESS_KEY");  // fornecido pela integração
        string SECRET_KEY = Environment.GetEnvironmentVariable("SECRET_KEY");  // fornecido pela integração
        string ID_PDV = Environment.GetEnvironmentVariable("ID_PDV");          // fornecido pela integração

        string input = $"{PRODUTO};{SUB_PRODUTO};{ACCESS_KEY};{SECRET_KEY};{ID_PDV}";
        var output = EmbedLib.Configurar(input);  // Supondo que 'EmbedLib' seja a biblioteca correspondente
        Console.WriteLine($"configurar = {output}");
        Log(output, "configurar");
    }

    public static int Iniciar()
    {
        string OPERACAO = "xml";  // produto para processamento
        var output = EmbedLib.Iniciar(OPERACAO);
        Console.WriteLine($"iniciar = {output}");
        Log(output, "iniciar");

        int result = int.Parse(EmbedLib.ObterValor(output, STATUS_CODE));
        return result;
    }

    public static int Zip(string pathZip)
    {
        string OPERACAO = "enviar_xml";  // operação para realizar envio de xml
        string TIPO_ENVIO = "zip";       // tipo do envio de xml
        string VALOR = pathZip;          // conteúdo/path para envio

        string input = $"{OPERACAO};{TIPO_ENVIO};{VALOR}";
        var output = EmbedLib.Processar(input);
        Console.WriteLine($"processar = {output}");
        Log(output, "zip");

        int result = int.Parse(EmbedLib.ObterValor(output, STATUS_CODE));
        return result;
    }

    public static int Rar(string pathRar)
    {
        string OPERACAO = "enviar_xml";  // operação para realizar envio de xml
        string TIPO_ENVIO = "rar";       // tipo do envio de xml
        string VALOR = pathRar;          // conteúdo/path para envio

        string input = $"{OPERACAO};{TIPO_ENVIO};{VALOR}";
        var output = EmbedLib.Processar(input);
        Console.WriteLine($"processar = {output}");
        Log(output, "rar");

        int result = int.Parse(EmbedLib.ObterValor(output, STATUS_CODE));
        return result;
    }

    public static int Path(string pathFile)
    {
        string OPERACAO = "enviar_xml";  // operação para realizar envio de xml
        string TIPO_ENVIO = "path";      // tipo do envio de xml
        string VALOR = pathFile;         // conteúdo/path para envio

        string input = $"{OPERACAO};{TIPO_ENVIO};{VALOR}";
        var output = EmbedLib.Processar(input);
        Console.WriteLine($"processar = {output}");
        Log(output, "path");

        int result = int.Parse(EmbedLib.ObterValor(output, STATUS_CODE));
        return result;
    }

    public static int Xml(string content)
    {
        string OPERACAO = "enviar_xml";  // operação para realizar envio de xml
        string TIPO_ENVIO = "xml";       // tipo do envio de xml
        string VALOR = content;          // conteúdo/path para envio

        string input = $"{OPERACAO};{TIPO_ENVIO};{VALOR}";
        var output = EmbedLib.Processar(input);
        Console.WriteLine($"processar = {output}");
        Log(output, "xml");

        int result = int.Parse(EmbedLib.ObterValor(output, STATUS_CODE));
        return result;
    }

    public static int Status()
    {
        string OPERACAO = "get_status";  // obtem o status do pagamento
        var output = EmbedLib.Processar(OPERACAO);
        Console.WriteLine($"processar = {output}");
        Log(output, "status");

        int result = int.Parse(EmbedLib.ObterValor(output, STATUS_CODE));
        return result;
    }

    public static int Finalizar()
    {
        string OPERACAO = "";  // finaliza a API
        var output = EmbedLib.Finalizar(OPERACAO);
        Console.WriteLine($"finalizar = {output}");
        Log(output, "finalizar");

        int result = int.Parse(EmbedLib.ObterValor(output, STATUS_CODE));
        return result;
    }

    public static void Log(string message, string operation)
    {
        string logDirectory = "log";
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

