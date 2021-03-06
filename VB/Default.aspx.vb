﻿Imports DevExpress.Web
Imports System.IO


Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        If Not IsPostBack Then
            Session.Clear()
        End If
    End Sub
    Public ReadOnly Property FileList() As List(Of MySavedObjects)
        Get
            Dim list As List(Of MySavedObjects) = TryCast(Session("list"), List(Of MySavedObjects))
            If list Is Nothing Then
                list = New List(Of MySavedObjects)()
                For i As Integer = 0 To ASPxGridView1.VisibleRowCount - 1
                    list.Add(New MySavedObjects() With {.RowNumber = i})
                Next i
                Session("list") = list
            End If
            Return list
        End Get
    End Property
    Protected Sub ASPxUploadControl1_FileUploadComplete(ByVal sender As Object, ByVal e As DevExpress.Web.FileUploadCompleteEventArgs)
        If e.IsValid Then
            Dim fileName As String = ASPxGridView1.EditingRowVisibleIndex & e.UploadedFile.FileName
            Dim path As String = "~/Documents/" & fileName
            e.UploadedFile.SaveAs(Server.MapPath(path), True)
            FileList(ASPxGridView1.EditingRowVisibleIndex).Url = Page.ResolveUrl(path)
            FileList(ASPxGridView1.EditingRowVisibleIndex).FileName = fileName
            Session("list") = FileList
            e.CallbackData = fileName
        End If
    End Sub

    Protected Sub ASPxHyperLink_Load(ByVal sender As Object, ByVal e As EventArgs)
        Dim hpl As ASPxHyperLink = TryCast(sender, ASPxHyperLink)
        Dim c As GridViewDataItemTemplateContainer = TryCast(hpl.NamingContainer, GridViewDataItemTemplateContainer)
        If (Not String.IsNullOrWhiteSpace(FileList(c.VisibleIndex).FileName)) AndAlso (Not String.IsNullOrWhiteSpace(FileList(c.VisibleIndex).Url)) Then
            hpl.Text = FileList(c.VisibleIndex).FileName
            hpl.NavigateUrl = FileList(c.VisibleIndex).Url
        End If
    End Sub
    Protected Sub ASPxGridView1_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs)
        Throw New CustomExceptions.MyException("Data updates aren't allowed in online examples. Click the Cancel button to check how your file was uploaded.")
    End Sub
    Protected Sub ASPxGridView1_CustomErrorText(ByVal sender As Object, ByVal e As ASPxGridViewCustomErrorTextEventArgs)
        If TypeOf e.Exception Is CustomExceptions.MyException Then
            e.ErrorText = e.Exception.Message
        End If
    End Sub
    Protected Sub ASPxCallback1_Callback(ByVal source As Object, ByVal e As CallbackEventArgs)
        Dim fileName As String = e.Parameter
        For Each myObj As MySavedObjects In FileList
            If myObj.FileName = fileName Then
                myObj.Url = String.Empty
                myObj.FileName = myObj.Url
            End If
        Next myObj
        File.Delete(Server.MapPath("~/Documents/" & fileName))
        e.Result = "ok"
    End Sub
End Class