XmlSerializer 对象的Xml序列化和反序列化
这篇随笔对应的.Net命名空间是System.Xml.Serialization；文中的示例代码需要引用这个命名空间。
为什么要做序列化和反序列化？
.Net程序执行时，对象都驻留在内存中；内存中的对象如果需要传递给其他系统使用；或者在关机时需要保存下来以便下次再次启动程序使用就需要序列化和反序列化。
范围：本文只介绍xml序列化，其实序列化可以是二进制的序列化，也可以是其他格式的序列化。
看一段最简单的Xml序列化代码

class Program
{
    static void Main(string[] args)
    {
        int i = 10;
        //声明Xml序列化对象实例serializer
        XmlSerializer serializer = new XmlSerializer(typeof(int));
        //执行序列化并将序列化结果输出到控制台
        serializer.Serialize(Console.Out, i);
        Console.Read();
    }
}
上面代码对int i进行了序列化，并将序列化的结果输出到了控制台，输出结果如下

<?xml version="1.0" encoding="gb2312"?>
<int>10</int>
可以将上述序列化的xml进行反序列化，如下代码


static void Main(string[] args)
{
    using (StringReader rdr = new StringReader(@"<?xml version=""1.0"" encoding=""gb2312""?>
<int>10</int>"))
    {
        //声明序列化对象实例serializer
        XmlSerializer serializer = new XmlSerializer(typeof(int));
        //反序列化，并将反序列化结果值赋给变量i
        int i = (int)serializer.Deserialize(rdr);
        //输出反序列化结果
        Console.WriteLine("i = " + i);
        Console.Read();
    }
}
以上代码用最简单的方式说明了xml序列化和反序列化的过程，.Net系统类库为我们做了大量的工作，序列化和反序列化都非常简单。但是在现实中业务需求往往比较复杂，不可能只简单的序列化一个int变量，显示中我们需要对复杂类型进行可控制的序列化。

自定义对象的Xml序列化：

System.Xml.Serialization命名空间中有一系列的特性类，用来控制复杂类型序列化的控制。例如XmlElementAttribute、XmlAttributeAttribute、XmlArrayAttribute、XmlArrayItemAttribute、XmlRootAttribute等等。

看一个小例子，有一个自定义类Cat，Cat类有三个属性分别为Color，Saying，Speed。


namespace UseXmlSerialization
{
    class Program
    {
        static void Main(string[] args)
        {
            //声明一个猫咪对象
            var c = new Cat { Color = "White", Speed = 10, Saying = "White or black,  so long as the cat can catch mice,  it is a good cat" };
 
            //序列化这个对象
            XmlSerializer serializer = new XmlSerializer(typeof(Cat));
 
            //将对象序列化输出到控制台
            serializer.Serialize(Console.Out, c);
 
            Console.Read();
        }
    }
 
    [XmlRoot("cat")]
    public class Cat
    {
        //定义Color属性的序列化为cat节点的属性
        [XmlAttribute("color")]
        public string Color { get; set; }
 
        //要求不序列化Speed属性
        [XmlIgnore]
        public int Speed { get; set; }
 
        //设置Saying属性序列化为Xml子元素
        [XmlElement("saying")]
        public string Saying { get; set; }
    }
}
可以使用XmlElement指定属性序列化为子节点（默认情况会序列化为子节点）；或者使用XmlAttribute特性制定属性序列化为Xml节点的属性；还可以通过XmlIgnore特性修饰要求序列化程序不序列化修饰属性。

对象数组的Xml序列化：

数组的Xml序列化需要使用XmlArrayAttribute和XmlArrayItemAttribute；XmlArrayAttribute指定数组元素的Xml节点名，XmlArrayItemAttribute指定数组元素的Xml节点名。

如下代码示例：

/*玉开技术博客 http://www.cnblogs.com/yukaizhao */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
 
namespace UseXmlSerialization
{
    class Program
    {
        static void Main(string[] args)
        {
            //声明一个猫咪对象
            var cWhite = new Cat { Color = "White", Speed = 10, Saying = "White or black,  so long as the cat can catch mice,  it is a good cat" };
            var cBlack = new Cat { Color = "Black", Speed = 10, Saying = "White or black,  so long as the cat can catch mice,  it is a good cat" };
 
            CatCollection cc = new CatCollection { Cats = new Cat[] { cWhite,cBlack} };
 
            //序列化这个对象
            XmlSerializer serializer = new XmlSerializer(typeof(CatCollection));
 
            //将对象序列化输出到控制台
            serializer.Serialize(Console.Out, cc);
 
            Console.Read();
        }
    }
 
    [XmlRoot("cats")]
    public class CatCollection
    {
        [XmlArray("items"),XmlArrayItem("item")]
        public Cat[] Cats { get; set; }
    }
 
    [XmlRoot("cat")]
    public class Cat
    {
        //定义Color属性的序列化为cat节点的属性
        [XmlAttribute("color")]
        public string Color { get; set; }
 
        //要求不序列化Speed属性
        [XmlIgnore]
        public int Speed { get; set; }
 
        //设置Saying属性序列化为Xml子元素
        [XmlElement("saying")]
        public string Saying { get; set; }
    }
}
以上代码将输出：


<?xml version="1.0" encoding="gb2312"?>
<cats xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://ww
w.w3.org/2001/XMLSchema">
  <items>
    <item color="White">
      <saying>White or black,  so long as the cat can catch mice,  it is a good
cat</saying>
    </item>
    <item color="Black">
      <saying>White or black,  so long as the cat can catch mice,  it is a good
cat</saying>
    </item>
  </items>
</cats>
XmlSerializer内存泄漏问题：

多谢chenlulouis，仔细看了下msdn，确实存在泄漏的情况，msdn说明如下：

动态生成的程序集 

为了提高性能，XML 序列化基础结构将动态生成程序集，以序列化和反序列化指定类型。此基础结构将查找并重复使用这些程序集。此行为仅在使用以下构造函数时发生： 

XmlSerializer(Type) 
XmlSerializer.XmlSerializer(Type, String) 

如果使用任何其他构造函数，则会生成同一程序集的多个版本，且绝不会被卸载，这将导致内存泄漏和性能降低。最简单的解决方案是使用先前提到的两个构造函数的其中一个。否则，必须在 Hashtable 中缓存程序集，如以下示例中所示。

也就是说我们在使用XmlSerializer序列化，初始化XmlSerializer对象时最好使用下面两个构造函数否则会引起内存泄漏。
XmlSerializer(Type)
XmlSerializer.XmlSerializer(Type, String)