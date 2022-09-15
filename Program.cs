using net.sf.saxon.s9api;
using net.liberty_development.SaxonHE11s9apiExtensions;
using System.Reflection;

// force loading of updated xmlresolver
ikvm.runtime.Startup.addBootClassPathAssembly(Assembly.Load("org.xmlresolver.xmlresolver"));
ikvm.runtime.Startup.addBootClassPathAssembly(Assembly.Load("org.xmlresolver.xmlresolver_data"));

var processor = new Processor(false);

Console.WriteLine($"{processor.getSaxonEdition()} {processor.getSaxonProductVersion()}");

var xml = @"<items>
  <item>
   <name>item 1</name>
   <category>foo</category>
  </item>
  <item>
   <name>item 2</name>
   <category>bar</category>
  </item>
  <item>
   <name>item 3</name>
   <category>foo</category>
  </item>
</items>";

var xquery = @"
declare namespace map = 'http://www.w3.org/2005/xpath-functions/map';

declare namespace output = 'http://www.w3.org/2010/xslt-xquery-serialization';

declare option output:method 'json';
declare option output:indent 'yes';
map:merge(
    for $item in /items/item
    group by $cat := $item/category
    return 
      map {
        $cat : array { $item/name/string() }
      }
)";

var xqueryEvaluator = processor.newXQueryCompiler().compile(xquery).load();

xqueryEvaluator.setContextItem(processor.newDocumentBuilder().build(xml.AsSource()));

xqueryEvaluator.run(processor.NewSerializer(Console.Out));

