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
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

namespace MapperDslUI
{
    public class JsonMainWindowViewModel : INotifyPropertyChanged
    {
        private const string DEFAULT_DEFINITION = @"
reference -> Reference
jobDescription.jobTitle -> JobAdDetails.Title
jobDescription.description1 -> JobAdDetails.MissionDescription
jobDescription.description2 -> JobAdDetails.ProfileDescription
jobDescription.jobDescriptionCustomFields.customCodeTable1.label -> Location.Address
""42.90"" -> Location.Coordinates.Longitude
""-44.967"" -> Location.Coordinates.Latitude
";

        public event PropertyChangedEventHandler PropertyChanged;

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

        public JsonMainWindowViewModel()
        {
            OriginText.Text = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(ModelBuilder).Namespace}.VacancyDetailRead.json"), Encoding.UTF8).ReadToEnd();
            MappingDefinition.Text = DEFAULT_DEFINITION;
            ConsoleOutput = string.Empty;
            TargetText.TextChanged += Text_TextChanged;
            OriginText.TextChanged += Text_TextChanged;
            MappingDefinition.TextChanged += Text_TextChanged;
        }

        private void Text_TextChanged(object sender, EventArgs e)
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
                var origin = JsonNode.Parse(OriginText.Text);
                var mapper = new Mapper(functionProvider, new StringReader(MappingDefinition.Text));
                var (success, errors) = mapper.Load();
                if (!success)
                {
                    AppendToConsoleOutput(String.Join("\r\n", errors));
                    return;
                }
                var mapperHandler = mapper.GetMapperJsonToJson();
                var target = new JsonObject();
                mapperHandler.Map(origin, target);
                TargetText.Text = target.ToString();
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
            public SourceResult GetObject(VacancyDetailRead instanceObj, Parameters parameters)
            {
                return new SourceResult()
                {
                    Result = GetResults()
                };

                IEnumerable<object> GetResults()
                {
                    foreach (var item in parameters.Values[0].Result)
                    {
                        yield return $"{item}";
                    }
                }
            }
        }

        class GenerateId : IExtractFunctionHandler<VacancyDetailRead>
        {
            public SourceResult GetObject(VacancyDetailRead instanceObj, Parameters parameters)
            {
                return new SourceResult()
                {
                    Result = GetResults()
                };

                IEnumerable<object> GetResults()
                {
                    var hash = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes($"{parameters.Values[0].Result.FirstOrDefault()}_{DateTime.Now.ToString("s")}"))).Replace("-", "");
                    yield return $"{parameters.Values[0].Result.FirstOrDefault()}_{hash.Substring(0, 8).ToLower()}";
                }
            }
        }

        class AddProperty : IInsertFunctionHandler<JobAd>, IInsertTupleFunctionHandler<JobAd>
        {
            public void SetObject(JobAd instanceObject, SourceResult source, Parameters parameters)
            {
                foreach (var item in source.Result)
                {
                    SinglePropertyItem property = null;
                    switch (item)
                    {
                        case Reference reference:
                            property = new SinglePropertyItem { Id = reference.Id, Label = reference.Label + source.Name ?? string.Empty };
                            break;
                        default:
                            property = new SinglePropertyItem() { Label = item.ToString() };
                            break;
                    }
                    if (property != null)
                    {
                        if (!instanceObject.Properties.ContainsKey((string)parameters.Values[0].Result.First()))
                        {
                            instanceObject.Properties.Add((string)parameters.Values[0].Result.First(), new SingleProperty() { Items = new List<SinglePropertyItem>() { property } });
                        }
                        else
                        {
                            ((SingleProperty)instanceObject.Properties[(string)parameters.Values[0].Result.First()]).Items.Add(property);
                        }
                    }
                }
            }

            public void SetObject(JobAd instanceObject, TupleSourceResult source, Parameters parameters)
            {
                foreach (var tuple in source.Result)
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
                        _ => null,
                    };

                    if (property == null) continue;

                    if (!instanceObject.Properties.ContainsKey((string)parameters.Values[0].Result.First()))
                    {
                        instanceObject.Properties.Add((string)parameters.Values[0].Result.First(), property);
                    }
                    else
                    {
                        switch (property)
                        {
                            case SingleProperty singleProperty:
                                ((SingleProperty)instanceObject.Properties[(string)parameters.Values[0].Result.First()]).Items.Add(singleProperty.Items[0]);
                                break;
                            case CompositeProperty compositeProperty:
                                ((CompositeProperty)instanceObject.Properties[(string)parameters.Values[0].Result.First()]).Items.Add(compositeProperty.Items[0]);
                                break;
                        }

                    }
                }
                CompositeProperty BuildCompositeItem(List<object> tuple)
                {
                    CompositePropertyItem property = new CompositePropertyItem();
                    int index = 0;
                    foreach (var item in tuple)
                    {
                        property.Add(BuildSingleProperty(item, index));
                        ++index;
                    }
                    return new CompositeProperty() { Items = new List<CompositePropertyItem>() { property } };
                }
                SinglePropertyItem BuildSingleProperty(object obj, int index = 0)
                {
                    return obj switch
                    {
                        Reference reference => new SinglePropertyItem { Id = reference.Id, Label = reference.Label + source.TupleDataInfo[index].Name ?? string.Empty },
                        _ => new SinglePropertyItem() { Label = obj?.ToString() }
                    };
                }
            }
        }
    }
}
