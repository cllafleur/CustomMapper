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

        private string originText;

        public string OriginText
        {
            get { return originText; }
            set
            {
                originText = value;
                OnPropertyChanged(nameof(OriginText));
                DoMap();
            }
        }

        private string targetText;

        public string TargetText
        {
            get { return targetText; }
            set
            {
                targetText = value;
                OnPropertyChanged(nameof(TargetText));
                DoMap();
            }
        }

        private string mappingDefinition;

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

        private string consoleOutput;
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
            TargetText = SerializeObject(ModelBuilder.GetNewTargetObject());
            OriginText = SerializeObject(ModelBuilder.GetNewOriginObject());
            MappingDefinition = DEFAULT_DEFINITION;
            ConsoleOutput = string.Empty;
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
                var origin = DeserializeObject<OriginObject>(OriginText);
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
                TargetText = SerializeObject(target);
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
            return result;
        }

        private T DeserializeObject<T>(string text)
        {
            return JsonSerializer.Deserialize<T>(text);
        }

        private void AppendToConsoleOutput(string text)
        {
            ConsoleOutput = $"{ConsoleOutput}{text}\r\n";
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
