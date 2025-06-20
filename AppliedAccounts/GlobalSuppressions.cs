﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;


[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Only targeting Windows platform", Scope = "module")]
[assembly: SuppressMessage("Performance", "CA1862:Use the 'StringComparison' method overloads to perform case-insensitive string comparisons", Justification = "Only targeting Windows platform", Scope = "module")]