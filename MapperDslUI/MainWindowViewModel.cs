using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using MapperDslLib;
using MapperDslLib.Runtime;
using MapperDslUI.Models;
using MapperDslUI.Models.Origin;
using MapperDslUI.Models.Target;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

namespace MapperDslUI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private const string DEFAULT_DEFINITION = @"
Title -> Description.Title
Description -> Description.Description
ModificationDate -> ModificationDate
ExtractRef(Description) -> AddProperty(""contractType"")
            ";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public IHighlightingDefinition SyntaxHighlight => HighlightingManager.Instance.GetDefinition("Json");

        private TextDocument originText = new TextDocument();

        public TextDocument OriginText
        {
            get { return originText; }
            set
            {
                originText = value;
                OnPropertyChanged(nameof(OriginText));
                DoMap();
            }
        }

        private TextDocument targetText = new TextDocument();

        public TextDocument TargetText
        {
            get { return targetText; }
            set
            {
                targetText = value;
                OnPropertyChanged(nameof(TargetText));
                DoMap();
            }
        }

        private string mappingDefinition = string.Empty;

        public string MappingDefinition
        {
            get { return mappingDefinition; }
            set
            {
                mappingDefinition = value;
                OnPropertyChanged(nameof(MappingDefinition));
                DoMap();
            }
        }

        private string consoleOutput = string.Empty;
        private bool doingMapping;

        public string ConsoleOutput
        {
            get { return consoleOutput; }
            set
            {
                consoleOutput = value;
                OnPropertyChanged(nameof(ConsoleOutput));
            }
        }

        public MainWindowViewModel()
        {
            TargetText.Text = SerializeObject(ModelBuilder.GetNewTargetObject());
            OriginText.Text = SerializeObject(ModelBuilder.GetNewOriginObject());
            MappingDefinition = DEFAULT_DEFINITION;
            ConsoleOutput = string.Empty;
            TargetText.TextChanged += Text_TextChanged;
            OriginText.TextChanged += Text_TextChanged;
        }

        private void Text_TextChanged(object? sender, EventArgs e)
        {
            DoMap();
        }

        private void DoMap()
        {
            if (doingMapping)
            {
                return;
            }
            doingMapping = true;
            try
            {
                var functionProvider = new FunctionHandlerProvider();
                functionProvider.Register<IExtractFunctionHandler<OriginObject>, ExtractRef>("ExtractRef");
                functionProvider.Register<IInsertFunctionHandler<TargetObject>, AddProperty>("AddProperty");
                var origin = DeserializeObject<OriginObject>(OriginText.Text);
                var mapper = new Mapper(functionProvider, new StringReader(MappingDefinition));
                var (success, errors) = mapper.Load();
                if (!success)
                {
                    AppendToConsoleOutput(String.Join("\r\n", errors));
                    return;
                }
                var mapperHandler = mapper.GetMapper<OriginObject, TargetObject>();
                var target = ModelBuilder.GetNewTargetObject();
                mapperHandler.Map(origin, target);
                TargetText.Text = SerializeObject(target);
                AppendToConsoleOutput(string.Empty);
            }
            catch (MapperRuntimeException ex)
            {
                AppendToConsoleOutput(GetLogFromException(ex));
            }
            catch (Exception ex)
            {
                AppendToConsoleOutput(ex.Message);
            }
            finally
            {
                doingMapping = false;
            }
        }

        private string SerializeObject<T>(T obj)
        {
            var result = JsonSerializer.Serialize(obj, new JsonSerializerOptions() { WriteIndented = true });
            return result ?? string.Empty;
        }

        private T DeserializeObject<T>(string text)
            where T : new()
        {
            return JsonSerializer.Deserialize<T>(text) ?? new T();
        }

        private string GetLogFromException(Exception exc)
        {
            string message = string.Empty;
            switch (exc)
            {
                case MapperRuntimeException mapperExc:
                    message = $"Line: {mapperExc.ParsingInfo.Line} {mapperExc.ParsingInfo.Text} {exc.Message}";
                    break;
                default:
                    message = exc.Message;
                    break;
            }
            string innerMessage = string.Empty;
            if (exc.InnerException != null)
            {
                innerMessage = GetLogFromException(exc.InnerException);
            }
            return $"{innerMessage}\r\n{message}";
        }

        private void AppendToConsoleOutput(string text)
        {
            ConsoleOutput = $"{text}\r\n";
        }

        class ExtractRef : IExtractFunctionHandler<OriginObject>
        {
            public object GetObject(OriginObject instanceObj, params object[] args)
            {
                return $"{args[0]}__new";
            }
        }

        class AddProperty : IInsertFunctionHandler<TargetObject>
        {
            public void SetObject(TargetObject instanceObject, object value, params object[] args)
            {
                instanceObject.Properties.Add((string)args[0], (string)value);
            }
        }
    }
}
