using System;
using NExpect.Implementations;
using NExpect.Interfaces;
using NExpect.MatcherLogic;
using PeanutButter.Utils;

namespace Ghoul.Tests
{
    public static class TypeMatchers
    {
        public static void Of(this IInstanceContinuation continuation, Type expected)
        {
            continuation.AddMatcher(actual =>
            {
                var passed = actual.IsAssignableFrom(expected);
                return new MatcherResult(
                    passed,
                    $"Expected {actual.PrettyName()} {passed.AsNot()}to be an instance of {expected.PrettyName()}"
                );
            });
        }
    }
}