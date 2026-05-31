// Compile-time polyfills that let this netstandard2.1 library use modern C# features whose
// supporting attributes ship only in .NET 5/7+. All internal, no runtime cost, Unity-safe.
namespace System.Runtime.CompilerServices
{
    /// <summary>Enables <c>init</c> accessors and records (C# 9).</summary>
    internal static class IsExternalInit { }

    /// <summary>Enables the <c>required</c> modifier (C# 11).</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute { }

    /// <summary>Emitted alongside <c>required</c> members (C# 11).</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName) => FeatureName = featureName;

        public string FeatureName { get; }
        public bool IsOptional { get; init; }

        public const string RefStructs = nameof(RefStructs);
        public const string RequiredMembers = nameof(RequiredMembers);
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>Lets a constructor declare it sets all <c>required</c> members (C# 11).</summary>
    [AttributeUsage(AttributeTargets.Constructor, Inherited = false)]
    internal sealed class SetsRequiredMembersAttribute : Attribute { }
}
