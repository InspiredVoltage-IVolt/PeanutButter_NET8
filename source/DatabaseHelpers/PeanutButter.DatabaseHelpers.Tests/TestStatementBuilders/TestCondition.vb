﻿Imports NUnit.Framework
Imports PeanutButter.DatabaseHelpers.StatementBuilders
Imports PeanutButter.RandomGenerators
Imports PeanutButter.Utils
Imports NExpect
Imports NExpect.Expectations

Namespace TestStatementBuilders
    Public Class TestCondition
        <Test()>
        Public Sub Constructor_GivenFieldAndValue_ProducesExpectedCondition()
            Dim fld = RandomValueGen.GetRandomString()
            Dim value = RandomValueGen.GetRandomString()
            Dim c = New Condition(New SelectField(fld), Condition.EqualityOperators.Equals, value)
            Expect(c.ToString()) _
                .To.Equal("[" + fld + "]='" + value + "'")
        End Sub

        <Test()>
        Public Sub Constructor_GivenStringFieldAndNonQuotedValue_ProducesExpectedCondition()
            Dim fld = RandomValueGen.GetRandomString()
            Dim val = RandomValueGen.GetRandomInt()
            Dim c = New Condition(New SelectField(fld), Condition.EqualityOperators.Equals, val.ToString(), False)
            Expect(c.ToString()) _
                .To.Equal("[" + fld + "]=" + val.ToString())
        End Sub

        <Test()>
        Public Sub Constructor_GivenSelectFieldInsteadOfFieldNameForInt64_ShouldProduceSameResults()
            Dim fld = RandomValueGen.GetRandomString()
            Dim val = RandomValueGen.GetRandomInt()
            Dim expected = new Condition(new SelectField(fld), Condition.EqualityOperators.Equals, val.ToString(), False)
            Dim sut = New Condition(New SelectField(fld), Condition.EqualityOperators.Equals, val)
            Expect(sut.ToString()).To.Equal(expected.ToString())
        End Sub

        <Test()>
        Public Sub Constructor_GivenSelectFieldInsteadOfFieldNameForDecimal_ShouldProduceSameResults()
            Dim fld = RandomValueGen.GetRandomString()
            Dim val = RandomValueGen.GetRandomDecimal()
            Dim expected = new Condition(new SelectField(fld), Condition.EqualityOperators.Equals,
                                         new DecimalDecorator(val).ToString(), False)
            Dim sut = New Condition(New SelectField(fld), Condition.EqualityOperators.Equals, val)
            Expect(sut.ToString()) _
                .To.Equal(expected.ToString())
        End Sub

        <Test()>
        Public Sub Constructor_GivenSelectFieldInsteadOfFieldNameForNullableDecimal_ShouldProduceSameResults()
            Dim fld = RandomValueGen.GetRandomString()
            Dim val as Nullable(Of Decimal) = RandomValueGen.GetRandomDecimal()
            Dim expected = new Condition(new SelectField(fld), Condition.EqualityOperators.Equals,
                                         new DecimalDecorator(Convert.ToDecimal(val)).ToString(), False)
            Dim test = New Condition(New SelectField(fld), Condition.EqualityOperators.Equals, val)
            Expect(test.ToString()) _
            .To.Equal(expected.ToString())
        End Sub

        <Test()>
        Public Sub Constructor_GivenSelectFieldInsteadOfFieldNameForDouble_ShouldProduceSameResults()
            Dim fld = RandomValueGen.GetRandomString()
            Dim val = RandomValueGen.GetRandomDouble()
            Dim expected = new Condition(new SelectField(fld), Condition.EqualityOperators.Equals, val.ToString(), False)
            Dim test = New Condition(New SelectField(fld), Condition.EqualityOperators.Equals, val)
            Expect(test.ToString()) _
                .To.Equal(expected.ToString())
        End Sub

        <Test()>
        Public Sub Constructor_GivenSelectFieldInsteadOfFieldNameForDateTime_ShouldProduceSameResults()
            Dim fld = RandomValueGen.GetRandomString()
            Dim val = RandomValueGen.GetRandomDate()
            Dim expected = new Condition(new SelectField(fld), Condition.EqualityOperators.Equals,
                                         val.ToString("yyyy/MM/dd HH:mm:ss"), True)
            Dim test = New Condition(New SelectField(fld), Condition.EqualityOperators.Equals, val)
            Expect(test.ToString()) _
            .To.Equal(expected.ToString())
        End Sub

        <Test()>
        Public Sub Constructor_GivenTwoBasicFields_ProducesExpectedCondition()
            Dim t1 = RandomValueGen.GetRandomString(),
                f1 = RandomValueGen.GetRandomString()
            Dim t2 = RandomValueGen.GetRandomString(),
                f2 = RandomValueGen.GetRandomString()
            Dim leftField As SelectField = New SelectField(t1, f1)
            Dim rightField As SelectField = New SelectField(t2, f2)
            Dim c = New Condition(leftField, Condition.EqualityOperators.Equals, rightField)
            Expect(c.ToString()) _
                .To.Equal(leftField.ToString() + "=" + rightField.ToString())
        End Sub

        <Test()>
        Public Sub Constructor_GivenTwoBasicFieldsOnly_InfersEquality()
            Dim t1 = RandomValueGen.GetRandomString(),
                f1 = RandomValueGen.GetRandomString()
            Dim t2 = RandomValueGen.GetRandomString(),
                f2 = RandomValueGen.GetRandomString()
            Dim leftField As SelectField = New SelectField(t1, f1)
            Dim rightField As SelectField = New SelectField(t2, f2)
            Dim c = New Condition(leftField, rightField)
            Expect(c.ToString()) _
                .To.Equal(leftField.ToString() + "=" + rightField.ToString())
        End Sub

        <Test()>
        Public Sub Constructor_GivenFieldNameAndValueOnly_InfersEqualityOperator()
            Dim fld = RandomValueGen.GetRandomString()
            Dim value = RandomValueGen.GetRandomString()
            Dim c = New Condition(fld, value)
            Expect(c.ToString()) _
                .To.Equal("[" + fld + "]='" + value + "'")
        End Sub

        <Test()>
        Public Sub Constructor_GivenFieldAndValueOnly_InfersEqualityOperator()
            Dim t1 = RandomValueGen.GetRandomString(),
                f1 = RandomValueGen.GetRandomString(),
                val = RandomValueGen.GetRandomString()
            Dim leftField = New SelectField(t1, f1)
            Dim c = New Condition(leftField, val)
            Expect(c.ToString()) _
                .To.Equal(leftField.ToString() + "='" + val + "'")
        End Sub

        <Test()>
        Public Sub Int16ValueConstructor()
            Dim f = RandomValueGen.GetRandomString(),
                v = Int16.Parse(CStr(RandomValueGen.GetRandomInt(1, 100))),
                c = New Condition(f, Condition.EqualityOperators.Equals, v)
            Expect(c.ToString()) _
                .To.Equal("[" + f + "]=" + v.ToString())
        End Sub

        <Test()>
        Public Sub Int32ValueConstructor()
            Dim f = RandomValueGen.GetRandomString(),
                v = Int32.Parse(CStr(RandomValueGen.GetRandomInt(1, 100))),
                c = New Condition(f, Condition.EqualityOperators.Equals, v)
            Expect(c.ToString()) _
                .To.Equal("[" + f + "]=" + v.ToString())
        End Sub

        <Test()>
        Public Sub Int64ValueConstructor()
            Dim f = RandomValueGen.GetRandomString(),
                v = Int64.Parse(CStr(RandomValueGen.GetRandomInt(1, 100))),
                c = New Condition(f, Condition.EqualityOperators.Equals, v)
            Expect(c.ToString()) _
                .To.Equal("[" + f + "]=" + v.ToString())
        End Sub

        <Test()>
        Public Sub DecimalValueConstructor()
            Dim f = RandomValueGen.GetRandomString(),
                v = Decimal.Parse(CStr(RandomValueGen.GetRandomInt(1, 100))),
                c = New Condition(f, Condition.EqualityOperators.Equals, v)
            Expect(c.ToString()) _
                .To.Equal("[" + f + "]=" + v.ToString())
        End Sub

        <Test()>
        Public Sub DoubleValueConstructor()
            Dim f = RandomValueGen.GetRandomString(),
                v = Double.Parse(CStr(RandomValueGen.GetRandomInt(1, 100))),
                c = New Condition(f, Condition.EqualityOperators.Equals, v)
            Expect(c.ToString()) _
                .To.Equal("[" + f + "]=" + v.ToString())
        End Sub

        <Test()>
        Public Sub DateTimeValueConstructor()
            Dim f = RandomValueGen.GetRandomString(),
                v = RandomValueGen.GetRandomDate(),
                c = New Condition(f, Condition.EqualityOperators.Equals, v)
            Expect(c.ToString()) _
                .To.Equal("[" + f + "]='" + v.ToString("yyyy/MM/dd HH:mm:ss") + "'")
        End Sub

        <Test()>
        Public Sub NullString_WithEquality()
            Dim f = RandomValueGen.GetRandomString(),
                v = DirectCast(Nothing, String)
            Dim c = New Condition(f, Condition.EqualityOperators.Equals, v)
            Expect(c.ToString()) _
                .To.Equal("[" + f + "] is NULL")
        End Sub

        <Test()>
        Public Sub NullString_WithInequality()
            Dim f = RandomValueGen.GetRandomString(),
                v = DirectCast(Nothing, String)
            Dim c = New Condition(f, Condition.EqualityOperators.NotEquals, v)
            Expect(c.ToString()) _
                .To.Equal("[" + f + "] is NOT NULL")
        End Sub

        <Test()>
        Public Sub ConditionWithOneSelectField_RespectsDatabaseProvider
            Dim f = RandomValueGen.GetRandomString(),
                v = RandomValueGen.GetRandomString()
            Dim c = new Condition(new SelectField(f), v)
            Expect(c.ToString()) _
                .To.Equal("[" + f + "]='" + v + "'")
            c.UseDatabaseProvider(DatabaseProviders.Firebird)
            Expect(c.ToString()) _
                .To.Equal("""" + f + """='" + v + "'")
        End Sub

        <Test()>
        Public Sub ConditionWithSelectFields_RespectsDatabaseProvider
            Dim f1 = RandomValueGen.GetRandomString(),
                f2 = RandomValueGen.GetRandomString()
            Dim c = new Condition(new SelectField(f1), new SelectField(f2))
            Expect(c.ToString()) _
                .To.Equal("[" + f1 + "]=[" + f2 + "]")
            c.UseDatabaseProvider(DatabaseProviders.Firebird)
            Expect(c.ToString()) _
                .To.Equal("""" + f1 + """=""" + f2 + """")
        End Sub
    End Class
End NameSpace