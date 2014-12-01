using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TSharp
{
    class Program
    {
        class Token
        {
            public string tokenType;
            public string value;
            public Match match;
            public override string ToString()
            {
                return value;
            }
        }
        class TokenMatch
        {
            public Regex regex;
            public string tokenType;
            public Func<string, string> tokenAlterer = null;
        }
        static string RegexMatch(TokenMatch tokenMatch, string input)
        {
            string result = tokenMatch.regex.Match(input).Value;
            if (result!="")
                if (tokenMatch.tokenAlterer != null)
                    result = tokenMatch.tokenAlterer(result);
            return result+";";
        }
        static List<Token> LexicalAnalyzer(List<TokenMatch> tokenMatches, string input)
        {
            string newInput = input;//.Replace("\n", " \n") + " ";
            List<Token> tokens = new List<Token>();
            int index = 0;
            while (index < newInput.Length)
            {
                for (int i = 0; i < tokenMatches.Count; i++)
                {
                    Match match = tokenMatches[i].regex.Match(newInput, index);
                    if (match.Success && match.Index == index)
                    {
                        if (tokenMatches[i].tokenType == "whitespace" || tokenMatches[i].tokenType == "comment")
                        {
                            index = match.Index + match.Length;
                            break;
                        }
                        tokens.Add(new Token { match = match, value = tokenMatches[i].tokenAlterer != null ? tokenMatches[i].tokenAlterer(match.Value) : match.Value, tokenType = tokenMatches[i].tokenType });
                        //if (tokens[tokens.Count-1].tokenType!="string")
                        //    index = match.Index + tokens[tokens.Count - 1].value.Length;
                        //else
                        //    index = match.Index + match.Length;//tokens[tokens.Count - 1].value.Length
                        if (tokens[tokens.Count - 1].tokenType == "whitespace")
                            index = match.Index + match.Length;
                        else
                            index = match.Index + match.Value.Trim().Length;
                        break; 
                    }
                }
            }
            return tokens;
        }
        static void PrintLexicalAnalysis(List<Token> tokens, System.IO.TextWriter writer)
        {
            foreach (Token token in tokens)
            {
                if (token.tokenType!="whitespace")
                writer.WriteLine("{0}:({1}, {2}, {3})", token.tokenType, token.value, token.match.Index, token.match.Length);
            }
        }
        static List<TokenMatch> BuildTSharpLexer()
        {
            List<TokenMatch> tokenMatches = new List<TokenMatch>();

            tokenMatches.Add(new TokenMatch { regex = new Regex("%.*\\n"), tokenType = "comment", tokenAlterer = (x) => x.Substring(0, x.Length - 1) });
            tokenMatches.Add(new TokenMatch { regex = new Regex("var "), tokenType = "var", tokenAlterer = (x) => x.Substring(0, x.Length - 1) });
            tokenMatches.Add(new TokenMatch { regex = new Regex("((int)|(string)|(real)|(boolean))[^a-zA-Z0-9]"), tokenType = "varType",
                                              tokenAlterer = (x) => x.Substring(0, x.Length - 1) });
            tokenMatches.Add(new TokenMatch { regex = new Regex("[0-9]*\\.[0-9]+"), tokenType = "real" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("[0-9]+"), tokenType = "int" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("((true)|(false))[^a-zA-Z0-9]"), tokenType = "boolean" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("\"[^\\n\"]+\""), tokenType = "stringLiteral", tokenAlterer = (x) => x.Substring(1, x.Length - 2) });
            tokenMatches.Add(new TokenMatch { regex = new Regex(":="), tokenType = "setEqual" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("="), tokenType = "isEqual" });
            tokenMatches.Add(new TokenMatch { regex = new Regex(":"), tokenType = "colon" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("+"), tokenType = "+" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("-"), tokenType = "-" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("/"), tokenType = "/" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("mod[^a-zA-Z0-9]"), tokenType = "mod" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("**"), tokenType = "**" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("*"), tokenType = "*" });
            tokenMatches.Add(new TokenMatch { regex = new Regex(","), tokenType = "comma" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("put"), tokenType = "put" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("get"), tokenType = "get" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("\\("), tokenType = "leftParenth" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("\\)"), tokenType = "rightParenth" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("[a-zA-Z_][a-zA-Z0-9_]+"), tokenType = "word" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("\\n+"), tokenType = "newline" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("[ \\n\\r\\t]+"), tokenType = "whitespace" });
            tokenMatches.Add(new TokenMatch { regex = new Regex("."), tokenType = "unMatchedChar" });

            return tokenMatches;
        }
        static string OldConvertToCSharp(List<Token> tokens)
        {
            string output = "";
            if (tokens.Where((x) => x.tokenType == "var").FirstOrDefault() != null)
            {
                if (tokens.Where((x) => x.tokenType == "varType").FirstOrDefault() != null)
                    output = String.Format("{0} {1};", tokens.Where((x) => x.tokenType == "varType").FirstOrDefault().value, tokens.Where((x) => x.tokenType == "word").FirstOrDefault().value);
                else
                {
                    if (tokens.Where((x) => x.tokenType == "int").FirstOrDefault() != null)
                        output = String.Format("int {0} = {1};", tokens.Where((x) => x.tokenType == "word").FirstOrDefault().value, tokens.Where((x) => x.tokenType == "int").FirstOrDefault().value);
                    else if (tokens.Where((x) => x.tokenType == "double").FirstOrDefault() != null)
                        output = String.Format("double {0} = {1};", tokens.Where((x) => x.tokenType == "word").FirstOrDefault().value, tokens.Where((x) => x.tokenType == "double").FirstOrDefault().value);
                    else if (tokens.Where((x) => x.tokenType == "string").FirstOrDefault() != null)
                        output = String.Format("string {0} = {1};", tokens.Where((x) => x.tokenType == "word").FirstOrDefault().value, tokens.Where((x) => x.tokenType == "string").FirstOrDefault().value);
                }
            }
            return output;
        }
        static Dictionary<string, string> CreatePredefinedFunctions()
        {
            Dictionary<string, string> predefinedfunctions = new Dictionary<string, string>();

            return predefinedfunctions;
        }
        class TSharpParsingException : Exception
        {
            public Token token;
            public string errorType;
            public static Dictionary<string,string> errorMessages=new Dictionary<string,string>{
                        {"varNotFirst","Unexpected \"var\" encountered. \"var\" should only be used at the beginning of a line."}};
            public TSharpParsingException(string errorType, Token token)//:base(errorMessages[errorType])
            {
                this.token = token;
                this.errorType=errorType;
            }
        }
        static string ConvertToCSharp(List<Token> tokens)
        {
            bool isVarDeclaration = false;
            string varName = "";
            string varType = "";
            bool isRightSide = false;
            string line = "";
            for (int i = 0; i < tokens.Count; i++)
            {
                switch (tokens[i].tokenType)
                {
                    case "var":
                        if (i != 0)
                            throw new TSharpParsingException("varNotFirst", tokens[i]);
                        isVarDeclaration = true;
                        break;
                    case "word":
                        if (isVarDeclaration)
                        {
                            if (varName != "")
                                throw new TSharpParsingException("multipleVarNames", tokens[i]);
                        }
                        break;
                }
            }
            return line;
        }
        static List<Token> RemoveWhiteSpace(List<Token> inTokens)
        {
            List<Token> tokens = new List<Token>();
            foreach (Token token in inTokens)
            {
                if (token.tokenType != "whitespace" && token.tokenType != "comment")
                    tokens.Add(token);
            }
            return tokens;
        }
        static string ConvertToCSharpPreDef(List<Token> inTokens)
        {
            List<Token> tokens = RemoveWhiteSpace(inTokens);
            string line = "";
            int i = 0;
            string identifier;
            if (tokens.Count == 0)
                return "";
            switch (tokens[i].tokenType)
            {
                case "var":
                    i++;
                    if (tokens[i].tokenType != "word") throw new TSharpParsingException("expectedIdentifier", tokens[i]);
                    identifier = tokens[i].value;
                    i++;
                    switch (tokens[i].tokenType)
                    {
                        case "colon":
                            i++;
                            if (tokens[i].tokenType != "varType") throw new TSharpParsingException("expectedVarType", tokens[i]);
                            string varType = tokens[i].value;
                            line = String.Format("{0} {1};", varType, identifier);
                            break;
                        case "setEqual":
                            i++;
                            line = "var " + identifier + "=";
                            for (int p = i; p < tokens.Count; p++)
                            {
                                line += tokens[p].value;
                            }
                            line += ";";
                            break;
                        default:
                            throw new TSharpParsingException("unexpected", tokens[i]);
                    }
                    break;
                case "word":
                    identifier=tokens[i].value;
                    i++;
                    switch (tokens[i].tokenType)
                    {
                        case "setEqual":
                            i++;
                            line = identifier+"=";
                            for (int p = i; p < tokens.Count; p++)
                            {
                                line += tokens[p].value;
                            }
                            line += ";";
                            break;
                        case "leftParenth":
                            break;
                        case "word":
                            line = identifier + "(";
                            for (int p = i; p < tokens.Count; p++)
                            {
                                switch (tokens[p].tokenType)
                                {
                                    case "word":
                                        line += tokens[p].value+",";
                                        break;
                                    case "comma":
                                        break;
                                    default:
                                        throw new TSharpParsingException("unexpected", tokens[i]);
                                }
                            }
                            line = line.Substring(0, line.Length - 1) + ")";
                            break;
                    }
                    break;
                default:
                    throw new TSharpParsingException("unexpected", tokens[i]);
            }
            return line;
        }
        static string ConverToCSharp(List<Token> tokens)
        {
            //special cases
            Token leftSide = null;
            List<Token> rightSide = null;
            return "";
        }
        static void Main(string[] args)
        {
            //"var number:int:=.0\nvar text:string:=\"yo yo yo, this shouldn't match a var\""
            var file = new System.IO.StreamReader("test.t");
            var output = new System.IO.StreamWriter("test.cs");
            while (!file.EndOfStream)
            {
                string input = file.ReadToEnd() + " ";
                Console.WriteLine(input);
                PrintLexicalAnalysis(LexicalAnalyzer(BuildTSharpLexer(), input),output);
                //string line = ConvertToCSharpPreDef(LexicalAnalyzer(BuildTSharpLexer(), input));
                //Console.WriteLine(line);
                //output.WriteLine(line);
                Console.ReadKey();
                Console.WriteLine("==============================================");
            }
            file.Close();
            output.Close();
            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
