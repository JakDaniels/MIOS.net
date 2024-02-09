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
            var keyEnumValues = new Dictionary<string,Dictionary<string, List<string>>>();
            foreach (var section in iniData.Sections)
            {
                var sectionName = section.SectionName.Replace("\"", "");
                keyEnumValues.Add(sectionName, new Dictionary<string, List<string>>());
                foreach (var key in section.Keys)
                {
                    var keyValue = key.Value.Replace("\"", "").Split(';').First().Trim();
                    if (!keyEnumValues[sectionName].ContainsKey(key.KeyName)) keyEnumValues[sectionName].Add(key.KeyName, new List<string>());
                    keyEnumValues[sectionName][key.KeyName].Add(keyValue);
                    var comments = key.Comments.Select(c => c.Replace("\"", "").Trim()).ToList().Where(c => c != "").ToList();
                    foreach (var comment in comments)
                    {
                        var keyExample = comment.Split('=').Select(s => s.Trim()).ToList();
                        if (keyExample.Count == 2)
                        {
                            if (!keyEnumValues[sectionName].ContainsKey(keyExample[0])) keyEnumValues[sectionName].Add(keyExample[0], new List<string>());
                            keyEnumValues[sectionName][keyExample[0]].Add(keyExample[1]);
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
                sectionDto.NiceName = MakeNiceName(sectionName);
                foreach (var key in section.Keys)
                {
                    //Key Value
                    var keyValue = key.Value.Replace("\"", "").Split(';').First().Trim();

                    //Key Type
                    var keyType = InferValueType(keyValue);

                    //Key Nice Name (for UI)
                    var niceName = MakeNiceName(key.KeyName);

                    //Key Comments
                    var comments = key.Comments
                        .Select(c => c.Replace("\"", "").Replace(";", "").Trim())
                        .ToList()
                        .Where(c => c != "" && !c.Contains(" = "))
                        .ToList();

                    //Key Enum
                    var keyEnum = new List<string>();
                    if (keyEnumValues[sectionName].ContainsKey(key.KeyName)) keyEnum = keyEnumValues[sectionName][key.KeyName].Distinct().ToList();

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

            //add any keys we found in the comments but not in the keys
            //as inactive keys to the right section
            foreach (var section in config.Sections)
            {
                var sectionName = section.Key;
                foreach (var key in keyEnumValues[sectionName])
                {
                    if (section.Value.Keys.ContainsKey(key.Key)) continue;

                    //Key Type
                    var keyType = InferValueType(key.Value[0]);

                    //Key Nice Name (for UI)
                    var niceName = MakeNiceName(key.Key);

                    section.Value.Keys.Add(key.Key, new KeyDto
                    {
                        NiceName = niceName,
                        Comments = new List<string>(),
                        Value = key.Value.First(),
                        Type = keyType,
                        Enum = key.Value.Distinct().ToList(),
                        Active = false
                    });
                }
            }

            return config;
        }

        private string[] SplitUnderScore(string source) {
            return source.Split('_');
        }

        private string[] SplitCamelCase(string source) {
            return Regex.Split(source, @"(?<!^)(?=[A-Z])");
        }

        private string InferValueType(string source) {
            if (source.ToLower() == "true" || source.ToLower() == "false") return "bool";
            if (source.Contains('.') && float.TryParse(source, out _)) return "float";
            if (source.Contains('.') && double.TryParse(source, out _)) return "double";
            if (int.TryParse(source, out _)) return "int";
            return "string";
        }

        private string MakeNiceName(string source) {
            var niceName = source;
            var camelCase = SplitCamelCase(source);
            var underScore = SplitUnderScore(source);
            if(camelCase.Length > 1) niceName = string.Join(" ", camelCase);
            if(underScore.Length > 1) niceName = string.Join(" ", underScore.Select(s => s.ToUpper()[0] + s.Substring(1)));
            return Regex.Replace(niceName, @"(([A-Z]{1}) )", "$2").Trim();
        }

    }
}