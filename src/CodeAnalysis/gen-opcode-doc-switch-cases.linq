<Query Kind="Statements">
  <NuGetReference>LoxSmoke.DocXml</NuGetReference>
  <Namespace>DocXml.Reflection</Namespace>
  <Namespace>LoxSmoke.DocXml</Namespace>
  <Namespace>LoxSmoke.DocXml.Reflection</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Reflection.Emit</Namespace>
</Query>

var r = new DocXmlReader(@"C:\Program Files (x86)\Microsoft SDKs\NuGetPackages\System.Reflection.Primitives\4.0.1\ref\netstandard1.0\System.Reflection.Primitives.xml");

string QuotedOpCode(FieldInfo f) => $"\"{((OpCode)f.GetValue(null)).Name}\"";

typeof(System.Reflection.Emit.OpCodes)
    .GetFields()
    .OrderBy(f => f.Name)
    .Select(f => $"{QuotedOpCode(f),-16} => \"{r.GetMemberComment(f).Replace("\"", "\\\"")}\",")
    .Join("\n")
    .Dump();