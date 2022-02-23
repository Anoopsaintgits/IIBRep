namespace DotNetTransform
{
    using IBM.Broker.Plugin;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TransformNode : NBComputeNode
    {
        private static string notargetnamespace = "";

        private static void CopyMessageHeaders(NBElement inputRoot, NBElement outputRoot)
        {
            IEnumerator<NBElement> objA = inputRoot.GetEnumerator();
            try
            {
                while (true)
                {
                    if (!objA.MoveNext())
                    {
                        break;
                    }
                    NBElement current = objA.Current;
                    if (current.get_NextSibling() != null)
                    {
                        outputRoot.AddLastChild(current);
                    }
                }
            }
            finally
            {
                if (!object.ReferenceEquals(objA, null))
                {
                    objA.Dispose();
                }
            }
        }

        public override void Evaluate(NBMessageAssembly inputAssembly)
        {
            NBOutputTerminal terminal = base.OutputTerminal("Out");
            NBMessage message = inputAssembly.get_Message();
            using (NBMessage message2 = new NBMessage())
            {
                NBMessageAssembly assembly = new NBMessageAssembly(inputAssembly, message2);
                NBElement inputRoot = message.get_RootElement();
                NBElement outputRoot = message2.get_RootElement();
                CopyMessageHeaders(inputRoot, outputRoot);
                outputRoot.get_Element("Properties").get_Element("MessageSet").SetValue("DotNetLibrary");
                outputRoot.get_Element("Properties").get_Element("MessageType").SetValue("File");
                outputRoot.CreateLastChildUsingNewParser("XMLNSC");
                NBElement outputFileElement = outputRoot.get_Element("XMLNSC").CreateLastChild(notargetnamespace, "SaleEnvelope").CreateLastChild(notargetnamespace, "SaleList");
                IEnumerator<NBElement> objA = inputRoot.get_Element("XMLNSC").get_Element("SaleEnvelope").get_Element("SaleList").Children("Invoice").GetEnumerator();
                try
                {
                    while (true)
                    {
                        if (!objA.MoveNext())
                        {
                            break;
                        }
                        NBElement current = objA.Current;
                        TransformInvoice(outputFileElement, current);
                    }
                }
                finally
                {
                    if (!object.ReferenceEquals(objA, null))
                    {
                        objA.Dispose();
                    }
                }
                terminal.Propagate(assembly);
            }
        }

        private static void TransformInvoice(NBElement outputFileElement, NBElement inputInvoiceElement)
        {
            NBElement element = outputFileElement.CreateLastChild(notargetnamespace, "Statement");
            element.CreateFirstChild(0x3000100, "Style", "Full");
            element.CreateFirstChild(0x3000100, "Type", "Monthly");
            NBElement element2 = element.CreateLastChild(notargetnamespace, "Customer");
            string str = "";
            IEnumerator<NBElement> objA = inputInvoiceElement.Children("Initial").GetEnumerator();
            try
            {
                while (true)
                {
                    if (!objA.MoveNext())
                    {
                        break;
                    }
                    NBElement current = objA.Current;
                    str = str + ((string) current);
                }
            }
            finally
            {
                if (!object.ReferenceEquals(objA, null))
                {
                    objA.Dispose();
                }
            }
            element2.CreateLastChild(notargetnamespace, "Initials", str);
            element2.CreateLastChild(notargetnamespace, "Name", inputInvoiceElement.Children("Surname").First<NBElement>().GetString());
            element2.CreateLastChild(notargetnamespace, "Balance", inputInvoiceElement.Children("Balance").First<NBElement>().GetString());
            NBElement element4 = element.CreateLastChild(notargetnamespace, "Purchases");
            double num = 0.0;
            objA = inputInvoiceElement.Children("Item").GetEnumerator();
            try
            {
                while (true)
                {
                    if (!objA.MoveNext())
                    {
                        break;
                    }
                    NBElement current = objA.Current;
                    NBElement element6 = element4.CreateLastChild(notargetnamespace, "Article");
                    double num2 = current.get_Element("Price") * 1.6;
                    int num3 = current.get_Element("Quantity");
                    element6.CreateLastChild(notargetnamespace, "Desc", (string) current.get_Element("Description"));
                    element6.CreateLastChild(notargetnamespace, "Cost", (decimal) num2);
                    element6.CreateLastChild(notargetnamespace, "Qty", num3);
                    num += num2 * num3;
                }
            }
            finally
            {
                if (!object.ReferenceEquals(objA, null))
                {
                    objA.Dispose();
                }
            }
            element.CreateLastChild(notargetnamespace, "Amount", (decimal) num).CreateFirstChild(0x3000100, "Currency", inputInvoiceElement.Children("Currency").First<NBElement>().GetString());
        }
    }
}

