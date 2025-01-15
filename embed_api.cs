using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using dotenv.net;
using EmbedLib;  // Supondo que 'EmbedLib' seja a biblioteca correspondente

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

        int result = EmbedLib.ObterValor(output, STATUS_CODE);
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

        int result = EmbedLib.ObterValor(output, STATUS_CODE);
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

        int result = EmbedLib.ObterValor(output, STATUS_CODE);
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

        int result = EmbedLib.ObterValor(output, STATUS_CODE);
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

        int result = EmbedLib.ObterValor(output, STATUS_CODE);
        return result;
    }

    public static int Status()
    {
        string OPERACAO = "get_status";  // obtem o status do pagamento
        var output = EmbedLib.Processar(OPERACAO);
        Console.WriteLine($"processar = {output}");
        Log(output, "status");

        int result = EmbedLib.ObterValor(output, STATUS_CODE);
        return result;
    }

    public static int Finalizar()
    {
        string OPERACAO = "";  // finaliza a API
        var output = EmbedLib.Finalizar(OPERACAO);
        Console.WriteLine($"finalizar = {output}");
        Log(output, "finalizar");

        int result = EmbedLib.ObterValor(output, STATUS_CODE);
        return result;
    }

    public static void Log(string message, string operation)
    {
        string logDirectory = "log";
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        string logFileName = $"{operation}_{DateTime.Now:yyyyMMdd_HHmm}.txt";
        string logFilePath = Path.Combine(logDirectory, logFileName);

        File.AppendAllText(logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}");
    }

    public static void Main(string[] args)
    {
        // Teste das funções
        var arquivos = ListarArquivos("caminho/do/diretorio", "extensao");
        arquivos.ForEach(Console.WriteLine);
        
        Configurar();

        int statusCode = Iniciar();
        Console.WriteLine($"Status Code: {statusCode}");

        // Teste de novas funções
        int zipResult = Zip("caminho/do/zip");
        Console.WriteLine($"Zip Result: {zipResult}");

        int rarResult = Rar("caminho/do/rar");
        Console.WriteLine($"Rar Result: {rarResult}");

        int pathResult = Path("caminho/do/path");
        Console.WriteLine($"Path Result: {pathResult}");

        int xmlResult = Xml("conteudo_xml");
        Console.WriteLine($"Xml Result: {xmlResult}");

        int statusResult = Status();
        Console.WriteLine($"Status Result: {statusResult}");

        int finalizarResult = Finalizar();
        Console.WriteLine($"Finalizar Result: {finalizarResult}");
    }
}
