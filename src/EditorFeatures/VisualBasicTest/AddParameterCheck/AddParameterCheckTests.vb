﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.CodeRefactorings
Imports Microsoft.CodeAnalysis.Editor.VisualBasic.UnitTests.CodeRefactorings
Imports Microsoft.CodeAnalysis.VisualBasic.AddParameterCheck

Namespace Microsoft.CodeAnalysis.Editor.VisualBasic.UnitTests.AddParameterCheck
    Public Class AddParameterCheckTests
        Inherits AbstractVisualBasicCodeActionTest

        Protected Overrides Function CreateCodeRefactoringProvider(Workspace As Workspace, parameters As TestParameters) As CodeRefactoringProvider
            Return New VisualBasicAddParameterCheckCodeRefactoringProvider()
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestSimpleReferenceType() As Task
            Await TestInRegularAndScript1Async(
"
Imports System

class C
    public sub new([||]s as string)
    end sub
end class",
"
Imports System

class C
    public sub new(s as string)
        If s Is Nothing Then
            Throw New ArgumentNullException(NameOf(s))
        End If
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestNullable() As Task
            Await TestInRegularAndScript1Async(
"
Imports System

class C
    public sub new([||]i as integer?)
    end sub
end class",
"
Imports System

class C
    public sub new(i as integer?)
        If i Is Nothing Then
            Throw New ArgumentNullException(NameOf(i))
        End If
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestNotOnValueType() As Task
            Await TestMissingInRegularAndScriptAsync(
"
Imports System

class C
    public sub new([||]i as integer)
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestNotOnInterfaceParameter() As Task
            Await TestMissingInRegularAndScriptAsync(
"
Imports System

interface I
    sub M([||]s as string)
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestNotOnAbstractParameter() As Task
            Await TestMissingInRegularAndScriptAsync(
"
Imports System

class C
    mustoverride sub M([||]s as string)
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestDoNotUpdateExistingFieldAssignment() As Task
            Await TestInRegularAndScript1Async(
"
Imports System

class C
    private _s as string 

    public sub new([||]s as string)
        _s = s
    end sub
end class",
"
Imports System

class C
    private _s as string

    public sub new(s as string)
        If s Is Nothing Then
            Throw New ArgumentNullException(NameOf(s))
        End If

        _s = s
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestDoNotUpdateExistingPropertyAssignment() As Task
            Await TestInRegularAndScript1Async(
"
Imports System

class C
    private property S as string

    public sub new([||]s as string)
        Me.S = s
    end sub
end class",
"
Imports System

class C
    private property S as string

    public sub new(s as string)
        If s Is Nothing Then
            Throw New ArgumentNullException(NameOf(s))
        End If

        Me.S = s
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestInsertAfterExistingNullCheck1() As Task
            Await TestInRegularAndScript1Async(
"
Imports System

class C
    public sub new(a as string, [||]s as string)
        If a is nothing
        End If
    end sub
end class",
"
Imports System

class C
    public sub new(a as string, s as string)
        If a is nothing
        End If

        If s Is Nothing Then
            Throw New ArgumentNullException(NameOf(s))
        End If
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestInsertBeforeExistingNullCheck1() As Task
            Await TestInRegularAndScript1Async(
"
Imports System

class C
    public sub new([||]a as string, s as string)
        If s Is Nothing Then
        End If
    end sub
end class",
"
Imports System

class C
    public sub new(a as string, s as string)
        If a Is Nothing Then
            Throw New ArgumentNullException(NameOf(a))
        End If

        If s Is Nothing Then
        End If
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestMissingWithExistingNullCheck1() As Task
            Await TestMissingInRegularAndScriptAsync(
"
Imports System

class C
    public sub new([||]s as string)
        If s Is Nothing Then
            Throw New ArgumentNullException()
        End If
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestMissingWithExistingNullCheck3() As Task
            Await TestMissingInRegularAndScriptAsync(
"
Imports System

class C
    public sub new([||]s as string)
        If String.IsNullOrEmpty(s)
        End If
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestMissingWithExistingNullCheck4() As Task
            Await TestMissingInRegularAndScriptAsync(
"
Imports System

class C
    public sub new([||]s as string)
        If String.IsNullOrWhiteSpace(s)
        End If
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestInMethod() As Task
            Await TestInRegularAndScript1Async(
"
Imports System

class C
    sub F([||]s as string)
    end sub
end class",
"
Imports System

class C
    sub F(s as string)
        If s Is Nothing Then
            Throw New ArgumentNullException(NameOf(s))
        End If
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestNotOnLambdaParameter() As Task
            Await TestMissingInRegularAndScriptAsync(
"
Imports System

class C
    public sub new()
        dim f = function ([||]s as string)
                end function
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestSpecialStringCheck1() As Task
            Await TestInRegularAndScript1Async(
"
Imports System

class C
    public sub new([||]s as string)
    end sub
end class",
"
Imports System

class C
    public sub new(s as string)
        If String.IsNullOrEmpty(s) Then
            Throw New ArgumentException(""message"", NameOf(s))
        End If
    end sub
end class", index:=1)
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddParameterCheck)>
        Public Async Function TestSpecialStringCheck2() As Task
            Await TestInRegularAndScript1Async(
"
Imports System

class C
    public sub new([||]s as string)
    end sub
end class",
"
Imports System

class C
    public sub new(s as string)
        If String.IsNullOrWhiteSpace(s) Then
            Throw New ArgumentException(""message"", NameOf(s))
        End If
    end sub
end class", index:=2)
        End Function
    End Class
End Namespace