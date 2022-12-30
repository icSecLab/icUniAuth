<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="default.aspx.vb" Inherits="icUniAuth._default" %>
  <% 
      Response.Redirect(If(Page.Request.Item("redirect") Is Nothing OrElse Page.Request.Item("redirect").Trim = "", "/login", "/login?redirect=" & Page.Request.Item("redirect")))
  %>
