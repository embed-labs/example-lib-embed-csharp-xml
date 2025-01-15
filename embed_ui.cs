using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using EmbedApi; // Supondo que EmbedApi é a biblioteca correspondente em C#

public class Main : Form
{
    private Panel header;
    private Panel content;
    private Dictionary<string, Type> frames;

    public Main()
    {
        EmbedLib.Configurar();    // Configuração da API

        // Variáveis de cores
        Color corFundo = Color.Black;
        Color corBotao = Color.Green;
        Color corTexto = Color.White;

        this.Text = "Embed";

        // Responsividade
        this.Size = new Size(800, 600);
        this.BackColor = corFundo;

        header = new Panel()
        {
            Dock = DockStyle.Top,
            Height = this.ClientSize.Height / 5,
            BackColor = corFundo,
        };
        this.Controls.Add(header);

        content = new Panel()
        {
            Dock = DockStyle.Fill,
            BackColor = corFundo,
        };
        this.Controls.Add(content);

        frames = new Dictionary<string, Type>()
        {
            { "TelaPrincipal", typeof(TelaPrincipal) },
            { "TelaProcessamento", typeof(TelaProcessamento) },
        };
        MostrarFrame("TelaPrincipal");
    }

    public void MostrarFrame(string pageName)
    {
        if (content.Controls.Count > 0)
        {
            content.Controls.RemoveAt(0);
        }
        
        var frameType = frames[pageName];
        var frame = (Form)Activator.CreateInstance(frameType);
        frame.TopLevel = false;
        frame.Dock = DockStyle.Fill;
        content.Controls.Add(frame);
        frame.Show();
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Main());
    }
}

// Header Frame
public class HeaderFrame : Panel
{
    private Form parent;
    private int currentLogoIndex = 0;
    private Image[] logos;
    private PictureBox logoPictureBox;
    
    public HeaderFrame(Form parent)
    {
        this.parent = parent;

        logos = new Image[]
        {
            Image.FromFile("img/logo1.png"),
            Image.FromFile("img/logo2.png"),
            Image.FromFile("img/logo3.png")
        };

        logoPictureBox = new PictureBox
        {
            Image = logos[currentLogoIndex],
            SizeMode = PictureBoxSizeMode.AutoSize
        };

        this.Controls.Add(logoPictureBox);
        ToggleLogo();
    }

    private async void ToggleLogo()
    {
        while (true)
        {
            await Task.Delay(1000);
            currentLogoIndex = (currentLogoIndex + 1) % logos.Length;
            logoPictureBox.Image = logos[currentLogoIndex];
        }
    }
}

// Content Frame
public class ContentFrame : Panel
{
    private Form parent;
    private Form controller;

    public ContentFrame(Form parent)
    {
        this.parent = parent;
        this.controller = null;  // Será atribuído na chamada de MostrarFrame
    }

    public void MostrarFrame(Type frameClass)
    {
        if (controller != null)
        {
            controller.Close();
            controller.Dispose();
        }

        controller = (Form)Activator.CreateInstance(frameClass);
        controller.TopLevel = false;
        controller.Dock = DockStyle.Fill;
        this.Controls.Add(controller);
        controller.Show();
    }
}

// Tela Principal
public class TelaPrincipal : Panel
{
    private Form parent;
    private TextBox textbox;

    public TelaPrincipal(Form parent)
    {
        this.parent = parent;

        EmbedLib.Iniciar();   // Inicialização do produto XML

        Color corFundo = parent.BackColor;
        Color corBotao = Color.Green;
        Color corTexto = Color.White;

        Label label = new Label
        {
            Text = "Selecione o arquivo ou adicione o conteúdo:",
            BackColor = corFundo,
            ForeColor = corTexto,
            Font = new Font("Helvetica", 26),
            AutoSize = true
        };
        label.Dock = DockStyle.Top;
        this.Controls.Add(label);

        Panel entryFrame = new Panel
        {
            BackColor = corFundo,
            Dock = DockStyle.Top,
            Height = 50
        };
        this.Controls.Add(entryFrame);

        textbox = new TextBox
        {
            Font = new Font("Helvetica", 18),
            Dock = DockStyle.Fill
        };
        entryFrame.Controls.Add(textbox);

        Button browseButton = new Button
        {
            Text = "Browse",
            BackColor = corBotao,
            ForeColor = corTexto,
            Font = new Font("Helvetica", 18),
            Dock = DockStyle.Right
        };
        browseButton.Click += BrowseFile;
        entryFrame.Controls.Add(browseButton);

        Panel buttonFrame = new Panel
        {
            BackColor = corFundo,
            Dock = DockStyle.Top,
            Height = 70
        };
        this.Controls.Add(buttonFrame);

        buttonFrame.Controls.Add(CreateButton("ZIP", corBotao, corTexto, ProcessarZip));
        buttonFrame.Controls.Add(CreateButton("RAR", corBotao, corTexto, ProcessarRar));
        buttonFrame.Controls.Add(CreateButton("PATH", corBotao, corTexto, ProcessarPath));
        buttonFrame.Controls.Add(CreateButton("XML", corBotao, corTexto, ProcessarXml));
        buttonFrame.Controls.Add(CreateButton("Voltar", corBotao, corTexto, Voltar));
    }

    private Button CreateButton(string text, Color backColor, Color foreColor, EventHandler eventHandler)
    {
        Button button = new Button
        {
            Text = text,
            BackColor = backColor,
            ForeColor = foreColor,
            Font = new Font("Helvetica", 18),
            AutoSize = true
        };
        button.Click += eventHandler;
        return button;
    }

    private void BrowseFile(object sender, EventArgs e)
    {
        using (FolderBrowserDialog dialog = new FolderBrowserDialog())
        {
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textbox.Text = dialog.SelectedPath;
            }
        }
    }

    private void ProcessarZip(object sender, EventArgs e)
    {
        string valor = textbox.Text;
        Console.WriteLine("Caminho arquivo zip:", valor);

        string result = EmbedLib.Zip(valor);
        if (result == "1")
        {
            ((Main)parent).MostrarFrame("TelaProcessamento");
        }
    }

    private void ProcessarRar(object sender, EventArgs e)
    {
        string valor = textbox.Text;
        Console.WriteLine("Caminho arquivo rar:", valor);

        string result = EmbedLib.Rar(valor);
        if (result == "1")
        {
            ((Main)parent).MostrarFrame("TelaProcessamento");
        }
    }

    private void ProcessarPath(object sender, EventArgs e)
    {
        string valor = textbox.Text;
        Console.WriteLine("Caminho arquivo path:", valor);

        string result = EmbedLib.Path(valor);
        if (result == "1")
        {
            ((Main)parent).MostrarFrame("TelaProcessamento");
        }
    }

    private void ProcessarXml(object sender, EventArgs e)
    {
        string valor = textbox.Text;
        Console.WriteLine("Conteúdo xml:", valor);

        string result = EmbedLib.Xml(valor);
        if (result == "1")
        {
            ((Main)parent).MostrarFrame("TelaProcessamento");
        }
    }

    private void Voltar(object sender, EventArgs e)
    {
        ((Main)parent).MostrarFrame("TelaPrincipal");
    }
}

// Tela Processamento
public class TelaProcessamento : Panel
{
    private Form parent;
    private Label statusLabel;
    private ProgressBar spinner;
    private Button cancelButton;
    private Button voltarButton;

    public TelaProcessamento(Form parent)
    {
        this.parent = parent;

        Color corFundo = parent.BackColor;
        Color corBotao = Color.Green;
        Color corTexto = Color.White;

        Label label = new Label
        {
            Text = "Enviando XML para Datalake Embed",
            BackColor = corFundo,
            ForeColor = corTexto,
            Font = new Font("Helvetica", 18),
            AutoSize = true
        };
        label.Dock = DockStyle.Top;
        this.Controls.Add(label);

        spinner = new ProgressBar
        {
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 30
        };
        spinner.Dock = DockStyle.Top;
        this.Controls.Add(spinner);

        statusLabel = new Label
        {
            Text = "Aguardando processamento...",
            BackColor = corFundo,
            ForeColor = corTexto,
            Font = new Font("Helvetica", 18),
            AutoSize = true
        };
        statusLabel.Dock = DockStyle.Top;
        this.Controls.Add(statusLabel);

        Panel buttonFrame = new Panel
        {
            BackColor = corFundo,
            Dock = DockStyle.Top,
            Height = 70
        };
        this.Controls.Add(buttonFrame);

        cancelButton = CreateButton("Cancelar", corBotao, corTexto, Cancelar);
        voltarButton = CreateButton("Voltar", corBotao, corTexto, Voltar);

        buttonFrame.Controls.Add(cancelButton);
        buttonFrame.Controls.Add(voltarButton);

        Task.Run(() => Processar());
    }

    private Button CreateButton(string text, Color backColor, Color foreColor, EventHandler eventHandler)
    {
        Button button = new Button
        {
            Text = text,
            BackColor = backColor,
            ForeColor = foreColor,
            Font = new Font("Helvetica", 18),
            AutoSize = true
        };
        button.Click += eventHandler;
        return button;
    }

    private async Task Processar()
    {
        while (true)
        {
            string result = EmbedLib.Status();
            if (result == "0")
            {
                EmbedLib.Finalizar();

                this.Invoke((Action)(() =>
                {
                    this.Controls.Remove(statusLabel);
                    this.Controls.Remove(spinner);
                    this.Controls.Remove(cancelButton);
                    statusLabel.Text = "Envio realizado com sucesso!";
                    statusLabel.Font = new Font("Helvetica", 26);
                    this.Controls.Add(statusLabel);
                }));

                await Task.Delay(3000);
                Voltar(this, EventArgs.Empty);
                break;
            }

            await Task.Delay(1000); // Intervalo para checagem do status
        }
    }

    private void Cancelar(object sender, EventArgs e)
    {
        Console.WriteLine("Cancelando operação");
    }

    private void Voltar(object sender, EventArgs e)
    {
        ((Main)parent).MostrarFrame("TelaPrincipal");
    }
}
