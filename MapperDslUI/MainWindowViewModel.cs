using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using MapperDslLib;
using MapperDslLib.Runtime;
using MapperDslUI.Models;
using MapperDslUI.Models.Origin;
using MapperDslUI.Models.Target;
using MapperDslUI.Models.Target.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

namespace MapperDslUI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private const string DEFAULT_DEFINITION = @"
GenerateId(Reference) -> Reference
Reference -> AddProperty(""OfferReference"")
JobDescription.JobTitle -> JobAdDetails.Title
JobDescription.Description1 -> JobAdDetails.MissionDescription
JobDescription.Description2 -> JobAdDetails.ProfileDescription
Organisation -> AddProperty(""Organisation"")
JobDescription.Country -> AddProperty(""Country"")
JobDescription.JobDescriptionCustomFields.CustomCodeTable1.Label -> Location.Address
""42.90"" -> Location.Coordinates.Longitude
""-44.967"" -> Location.Coordinates.Latitude
JobDescription.ContractType -> AddProperty(""ContractType"")
Criteria.CriteriaCustomFields.LongText1 -> AddProperty(""InternalInformations"") # c'est un commentaire !
ExtractRef(CreationDate) -> AddProperty(""CreationDate"")
";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

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

        private TextDocument mappingDefinition = new TextDocument();

        public TextDocument MappingDefinition
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
            TargetText.Text = SerializeObject(ModelBuilder.GetNewJobAd(), false);
            OriginText.Text = SerializeObject(ModelBuilder.GetNewVacancyDetailRead(), false);
            MappingDefinition.Text = DEFAULT_DEFINITION;
            ConsoleOutput = string.Empty;
            TargetText.TextChanged += Text_TextChanged;
            OriginText.TextChanged += Text_TextChanged;
            MappingDefinition.TextChanged += Text_TextChanged;
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
                functionProvider.Register<IExtractFunctionHandler<VacancyDetailRead>, ExtractRef>("ExtractRef");
                functionProvider.Register<IInsertFunctionHandler<JobAd>, AddProperty>("AddProperty");
                functionProvider.Register<IExtractFunctionHandler<VacancyDetailRead>, GenerateId>("GenerateId");
                var origin = DeserializeObject<VacancyDetailRead>(OriginText.Text);
                var mapper = new Mapper(functionProvider, new StringReader(MappingDefinition.Text));
                var (success, errors) = mapper.Load();
                if (!success)
                {
                    AppendToConsoleOutput(String.Join("\r\n", errors));
                    return;
                }
                var mapperHandler = mapper.GetMapper<VacancyDetailRead, JobAd>();
                var target = ModelBuilder.GetNewJobAd();
                mapperHandler.Map(origin, target);
                TargetText.Text = SerializeObject(target, true);
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

        private string SerializeObject<T>(T obj, bool ignoreNull)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter() }
            };
            if (ignoreNull)
            {
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            }
            var result = JsonSerializer.Serialize<object>(obj, options);
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

        class ExtractRef : IExtractFunctionHandler<VacancyDetailRead>
        {
            public GetResult GetObject(VacancyDetailRead instanceObj, IEnumerable<object>[] args)
            {
                return new GetResult()
                {
                    Result = GetResults()
                };

                IEnumerable<object> GetResults()
                {
                    foreach (var item in args[0])
                    {
                        yield return $"{item}";
                    }
                }
            }
        }

        class GenerateId : IExtractFunctionHandler<VacancyDetailRead>
        {
            public GetResult GetObject(VacancyDetailRead instanceObj, IEnumerable<object>[] args)
            {
                return new GetResult()
                {
                    Result = GetResults()
                };

                IEnumerable<object> GetResults()
                {
                    var hash = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes($"{args[0].FirstOrDefault()}_{DateTime.Now.ToString("s")}"))).Replace("-", "");
                    yield return $"{args[0].FirstOrDefault()}_{hash.Substring(0, 8).ToLower()}";
                }
            }
        }

        class AddProperty : IInsertFunctionHandler<JobAd>, IInsertTupleFunctionHandler<JobAd>
        {
            public void SetObject(JobAd instanceObject, IEnumerable<object> value, params object[] args)
            {
                foreach (var item in value)
                {
                    SinglePropertyItem? property = null;
                    switch (item)
                    {
                        case Reference reference:
                            property = new SinglePropertyItem { Id = reference.Id, Label = reference.Label };
                            break;
                        default:
                            property = new SinglePropertyItem() { Label = item.ToString() };
                            break;
                    }
                    if (property != null)
                    {
                        if (!instanceObject.Properties.ContainsKey((string)args[0]))
                        {
                            instanceObject.Properties.Add((string)args[0], new SingleProperty() { Items = new List<SinglePropertyItem>() { property } });
                        }
                        else
                        {
                            ((SingleProperty)instanceObject.Properties[(string)args[0]]).Items.Add(property);
                        }
                    }
                }
            }

            public void SetObject(JobAd instanceObject, IEnumerable<IEnumerable<object>> value, params object[] args)
            {
                foreach (var tuple in value)
                {
                    var tupleValues = new List<object>();
                    foreach (var item in tuple)
                    {
                        tupleValues.Add(item);
                    }
                    Property property = tupleValues.Count switch
                    {
                        1 => new SingleProperty() { Items = new List<SinglePropertyItem>() { BuildSingleProperty(tupleValues[0]) } },
                        > 1 => BuildCompositeItem(tupleValues),
                    };


                    if (!instanceObject.Properties.ContainsKey((string)args[0]))
                    {
                        instanceObject.Properties.Add((string)args[0], property);
                    }
                    else
                    {
                        switch (property)
                        {
                            case SingleProperty singleProperty:
                                ((SingleProperty)instanceObject.Properties[(string)args[0]]).Items.Add(singleProperty.Items[0]);
                                break;
                            case CompositeProperty compositeProperty:
                                ((CompositeProperty)instanceObject.Properties[(string)args[0]]).Items.Add(compositeProperty.Items[0]);
                                break;
                        }

                    }
                }
                CompositeProperty BuildCompositeItem(List<object> tuple)
                {
                    CompositePropertyItem property = new CompositePropertyItem();
                    foreach (var item in tuple)
                    {
                        property.Add(BuildSingleProperty(item));
                    }
                    return new CompositeProperty() { Items = new List<CompositePropertyItem>() { property } };
                }
                SinglePropertyItem BuildSingleProperty(object obj)
                {
                    return obj switch
                    {
                        Reference reference => new SinglePropertyItem { Id = reference.Id, Label = reference.Label },
                        _ => new SinglePropertyItem() { Label = obj?.ToString() }
                    };
                }
            }
        }
    }
}
