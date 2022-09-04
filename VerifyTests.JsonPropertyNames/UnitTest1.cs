using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace VerifyTests.JsonPropertyNames;

[UsesVerify]
public class UnitTest1
{

    private readonly VerifySettings _verifySettings = new ();
    private readonly Regex _idFieldJsonRegex = new(@"^\t| *(?'idField'[\w]*Id): (?'idVal'[\w]*),?");
    //private readonly Regex _idFieldXmlRegex = new();
    
    public UnitTest1()
    {
        _verifySettings.ScrubLinesWithReplace(s => _idFieldJsonRegex.ReplaceGroupValue(s, "idVal", "IdentityValue"));
    }
    /// <summary>
    /// I expect the behaviour of ScrubLinesWithReplace to inject the full line into the delegate, including the property name, rather than the just value of the property on that line
    /// It doesn't, this test case will fail.
    /// I need to know the property name to accurately scrub out variable/changing values, like GUIDs and other Identity options. Not everyone uses GUIDs or Ints.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public Task Should_Remove_Id_Json_Lines_Using_Regex()
    {
        // Arrange
        var obj = new ExampleObject()
        {
            Id = GenerateRandomString(10),
            ForeignObjectId = GenerateRandomString(10)
        };
        
        // Act
        // Assert
        return Verify(obj, _verifySettings);
    }    
    
    public static string GenerateRandomString(int length, IEnumerable<char>? charSet = null)
    {
        charSet ??= "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var charArray = charSet.Distinct().ToArray();
        var result = new char[length];
        for (var i = 0; i < length; i++)
            result[i] = charArray[RandomNumberGenerator.GetInt32(charArray.Length)];
        return new string(result);
    }
}
[Serializable]
public record ExampleObject     
{
    public string Id { get; init; } = string.Empty;
    public string ForeignObjectId { get; init; } = string.Empty;
    
    public string Name { get; init; } = "Name Of Person";
} 