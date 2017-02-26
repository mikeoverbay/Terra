Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.IO
Imports System.Collections.Generic


Module ClinetModule

  Public Clients(30) As Client_
  Public Structure buffer_
    Public d() As Byte
    Public SpId As UInt32 'server packet Id
    Public CpId As UInt32 'Client packet Id - need this to check on resends
    Public originator As Boolean
    Public retry As Integer
  End Structure
  Public Class Client_
    Public lock_in As New Object
    'Public loct_out As New Object
    Public Id As Byte
    Private in_data(150) As Byte
    Private ori As Boolean
    Private out_data(150) As Byte
    Public client_name As String
    'Public outbuff() As buffer_
    Public inbuff() As buffer_
    Public trd As Thread
    Public ctrd As Thread
    Public ack_SpckId As UInteger
    Public ack_CpckId As UInteger
    Public ms As MemoryStream
    Public br As BinaryReader
    Public bw As BinaryWriter
    Public Sub start_up(ByVal id_ As UInt32)
      reset_()
      client_name = ""
      Id = id_
      trd = New Thread(AddressOf Me.reset_)
      ctrd = New Thread(AddressOf update_)
      trd.Name = "client:" + Id.ToString
      trd.Priority = ThreadPriority.Normal
      ctrd.Name = "ack_check:" + Id.ToString
      ctrd.Priority = ThreadPriority.Normal
      ctrd.Start()
    End Sub
    Public Sub data_in(ByVal b As Byte(), ByVal ori_ As Boolean)
      SyncLock lock_in
        b.CopyTo(in_data, 0)
        ori = ori_
      End SyncLock
    End Sub
    Public Sub ack_in(ByVal Sn_ As UInteger, ByVal Cn_ As UInteger)
      SyncLock lock_in
        ack_SpckId = Sn_
        ack_CpckId = Cn_
      End SyncLock
    End Sub
    Public Sub reset()
      client_name = ""
      reset_()
    End Sub
    Public Sub setup(ByVal n_ As String)
      reset_()
      client_name = n_
    End Sub
    Private Sub reset_()
      '---------------------------------------------
      ' ReDim outbuff(150)
      ReDim inbuff(150)
      For i = 0 To 150
        inbuff(i) = New buffer_
        inbuff(i).SpId = 0
        inbuff(i).CpId = 0
        inbuff(i).originator = False
        ReDim inbuff(i).d(150)
        inbuff(i).retry = 0
      Next     
    End Sub
    Private Sub update_()
      Dim Spack As UInteger
      Dim Cpack As UInteger
      '---------------------------------------------
      While 1
        '---------------------------------------------
        'process any ack's 
        SyncLock lock_in
          Spack = ack_SpckId
          Cpack = ack_CpckId
        End SyncLock
        If Spack > 0 Then
          For i = 0 To 150
            If inbuff(i).originator Then
              If Cpack = inbuff(i).CpId Then
                inbuff(i).CpId = 0
                inbuff(i).SpId = 0
                ack_SpckId = 0
                ack_CpckId = 0
              End If
            Else
              If Spack = inbuff(i).SpId Then
                inbuff(i).SpId = 0
                inbuff(i).CpId = 0
                SyncLock lock_in
                  ack_SpckId = 0
                  ack_CpckId = 0
                End SyncLock
              End If
            End If
          Next
        End If
        '---------------------------------------------
        Thread.Sleep(2)
        '---------------------------------------------
        ' add any new messages to the q
        ms = New MemoryStream(in_data)
        br = New BinaryReader(ms)
        Dim Sid_ = br.ReadUInt32
        Dim Cid_ = br.ReadUInt32
        If Sid_ > 0 Then ' we have a new packet in
          'first, see if this packed is in the q already
          SyncLock lock_in

            For i = 0 To 150
              If ori Then
                If inbuff(i).CpId = Cid_ Then
                  GoTo ignore_this_one
                End If
              Else
                If inbuff(i).SpId = Sid_ Then
                  GoTo ignore_this_one
                End If

              End If
            Next
          End SyncLock

          SyncLock lock_in
            For i = 0 To 150
              If inbuff(i).SpId = 0 Then
                inbuff(i).SpId = Sid_
                inbuff(i).CpId = Cid_
                inbuff(i).originator = ori
                in_data.CopyTo(inbuff(i).d, 0)
              End If
            Next
          End SyncLock
        End If
ignore_this_one:

        '---------------------------------------------
        ' kill threads if app is closing
        If form_closing Then
          trd.Abort()
          ctrd.Abort()
        End If
        '---------------------------------------------
        Thread.Sleep(2)
        '---------------------------------------------
      End While
      '---------------------------------------------
    End Sub

  End Class
End Module
