using System.Linq;
using System.Text.RegularExpressions;
using IniFileParser.Model;
using MIOS.net.Interfaces;
using MIOS.net.Models;

namespace MIOS.net.Services
{
    public class IniService: IIniService
    {
        public ConfigDto? GetDefaultIniData(string InstanceType)
        {
            var files = new List<string>();
            var iniData = new IniData();

            var r1 = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);






            switch (InstanceType)
            {
                case "Standalone":
                default:           
                    files = new List<string>{ 
                        "opensim/bin/OpenSimDefaults.ini", 
                        "opensim/bin/OpenSim.ini.example", 
                        "opensim/bin/config-include/Standalone.ini",
                        "opensim/bin/config-include/StandaloneCommon.ini.example",
                        "opensim/bin/config-include/FlotsamCache.ini.example",
                        "opensim/bin/config-include/osslEnable.ini"
                    };
                    break;
            }

            var dir = Path.GetDirectoryName(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])) ?? "";
            bool found = false;
            while (dir.Length > 3)
            {
                if(File.Exists(Path.GetFullPath(Path.Combine(dir, files.First()))))
                {
                    found = true;
                    break;
                }
                dir = Path.GetFullPath(Path.Combine(dir, "../"));
            }
            if (!found) return null;

            var parser = new IniFileParser.IniFileParser();
            foreach (var file in files)
            {
                var path = Path.GetFullPath(Path.Combine(dir, file));
                Console.WriteLine(path);
                if (File.Exists(path)) iniData.Merge(parser.ReadFile(path));
            }

            //now we have the iniData object, but some of the data needs cleaning up!!
            //Opensim configs are a mess, with comments and values on the same line, and comments with no key
            //and some keys are only defined in comments!

            //Scan the entire iniData Object for keys and commented out keys and build a list of keys
            //that might have a list of pre-defined values
            var keyEnumValues = new Dictionary<string, List<string>>();
            foreach (var section in iniData.Sections)
            {
                foreach (var key in section.Keys)
                {
                    var keyValue = key.Value.Replace("\"", "").Split(';').First().Trim();
                    if (!keyEnumValues.ContainsKey(key.KeyName)) keyEnumValues.Add(key.KeyName, new List<string>());
                    keyEnumValues[key.KeyName].Add(keyValue);
                    var comments = key.Comments.Select(c => c.Replace("\"", "").Trim()).ToList().Where(c => c != "").ToList();
                    foreach (var comment in comments)
                    {
                        var keyExample = comment.Split('=').Select(s => s.Trim()).ToList();
                        if (keyExample.Count == 2)
                        {
                            if (!keyEnumValues.ContainsKey(keyExample[0])) keyEnumValues.Add(keyExample[0], new List<string>());
                            keyEnumValues[keyExample[0]].Add(keyExample[1]);
                        }
                    }
                }
            }

            //now make a ConfigDto object for the UI
            var config = new ConfigDto();
            foreach (var section in iniData.Sections)
            {
                var sectionName = section.SectionName.Replace("\"", "");
                var sectionDto = new SectionDto();
                foreach (var key in section.Keys)
                {
                    //Key Value
                    var keyValue = key.Value.Replace("\"", "").Split(';').First().Trim();

                    //Key Type
                    var keyType = "string";
                    if (keyValue.ToLower() == "true" || keyValue.ToLower() == "false") keyType = "bool";                    
                    else
                    {
                        if (keyValue.Contains('.') && float.TryParse(keyValue, out _)) keyType = "float";
                        else
                        {
                            if (keyValue.Contains('.') && double.TryParse(keyValue, out _)) keyType = "double";
                            else
                            {
                                if (int.TryParse(keyValue, out _)) keyType = "int";                            
                            }
                        }
                    }

                    //Key Nice Name (for UI)
                    var niceName = key.KeyName;
                    var camelCase = SplitCamelCase(key.KeyName);
                    var underScore = SplitUnderScore(key.KeyName);
                    if(camelCase.Length > 1) niceName = string.Join(" ", camelCase);
                    if(underScore.Length > 1) niceName = string.Join(" ", underScore.Select(s => s.ToUpper()[0] + s.Substring(1)));

                    //Key Comments
                    var comments = key.Comments
                        .Select(c => c.Replace("\"", "").Replace(";", "").Trim())
                        .ToList()
                        .Where(c => c != "" && !c.Contains(" = "))
                        .ToList();

                    //Key Enum
                    var keyEnum = new List<string>();
                    if (keyEnumValues.ContainsKey(key.KeyName)) keyEnum = keyEnumValues[key.KeyName].Distinct().ToList();

                    sectionDto.Keys.Add(key.KeyName, new KeyDto
                    {
                        NiceName = niceName,
                        Comments = comments,
                        Value = keyValue,
                        Type = keyType,
                        Enum = keyEnum
                    });
                }
                config.Sections.Add(sectionName, sectionDto);
            }
            return config;
        }

        private string[] SplitUnderScore(string source) {
            return source.Split('_');
        }

        private string[] SplitCamelCase(string source) {
            return Regex.Split(source, @"(?<!^)(?=[A-Z])");
        }

    }
}