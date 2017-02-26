'DotZLib class library, a managed wrapper for the ZLIB.DLL compression library.

'Created by Alessandro Del Sole, 2008. The source code is provided "AS IS" with no implicit or
'explicit warranties. By using this code you agree that the Author is not responsible for the
'usage or damages deriving from the bad usage of the code itself.

'Contact: alessandro.delsole@visual-basic.it - http://community.visual-basic.it/Alessandro

'Password for the pfx file: dotzlib

Imports System
Imports System.Runtime.InteropServices

<Assembly: clscompliant(True)> 

Namespace DelSole.DotZLibCompressor

    ''' <summary>
    ''' Calls unmanaged APIs exposed by the ZLib.dll compression library
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class NativeMethods

        <DllImport("zlib.DLL", EntryPoint:="compress")> _
    Friend Shared Function CompressByteArray(ByVal dest As Byte(), ByRef destLen As Integer, ByVal src As Byte(), ByVal srcLen As Integer) As Integer
        End Function

        <DllImport("zlib.DLL", EntryPoint:="uncompress")> _
    Friend Shared Function UncompressByteArray(ByVal dest As Byte(), ByRef destLen As Integer, ByVal src As Byte(), ByVal srcLen As Integer) As Integer
        End Function

        Private Sub New()

        End Sub
    End Class

    ''' <summary>
    ''' Exposes methods and properties to store and retrieve compressed data
    ''' </summary>
    ''' <remarks></remarks>
    <CLSCompliant(True)> Public NotInheritable Class DotZLib

        Private Shared originalDim As Integer
        Private Shared compressedDim As Integer

        ''' <summary>
        ''' Returns data's uncompressed size
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property OriginalSize() As Integer
            Get
                Return originalDim
            End Get
        End Property

        ''' <summary>
        ''' Returns data's compressed size
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property CompressedSize() As Integer
            Get
                Return compressedDim
            End Get
        End Property

        ''' <summary>
        ''' Checks whenever ZLIB.DLL is installed in the System directory. Returns True or False
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property IsZLibInstalled() As Boolean
            Get
                Return IO.File.Exists(Environment.SystemDirectory & "\ZLIB.DLL")
            End Get
        End Property

        ''' <summary>
        ''' Compresses the specified bytes' array
        ''' </summary>
        ''' <param name="Data"></param>
        ''' <param name="TempBuffer"></param>
        ''' <remarks></remarks>
        Public Shared Sub CompressBytes(ByRef data() As Byte, Optional ByRef tempBuffer() As Byte = Nothing)

            'Compresses data to a temporary buffer
            'Stores compressed data in Data if TempBuff is not specified
            'If everything's alright, returns data's compressed size, otherwise -1

            'Original data size
            Dim OriginalSize As Long = UBound(data) + 1

            'Fields for the temporary buffer
            Dim result As Integer
            Dim usenewstorage As Boolean

            If tempBuffer Is Nothing Then usenewstorage = False Else usenewstorage = True

            'Resizes buffers
            Dim BufferSize As Integer = UBound(data) + 1
            BufferSize = CInt(BufferSize + (BufferSize * 0.01) + 12)
            ReDim tempBuffer(BufferSize)

            'Compresses bytes array of data
            result = NativeMethods.CompressByteArray(tempBuffer, BufferSize, data, UBound(data) + 1)

            'Verifies result
            If result = 0 Then
                If usenewstorage Then
                    'Returns TempBuffer's result
                    ReDim Preserve tempBuffer(BufferSize - 1)
                Else
                    'Returns compressed data in the bytes array
                    'and resizes data according to  compressed size
                    ReDim data(BufferSize - 1)
                    Array.Copy(tempBuffer, data, BufferSize)
                    'Releases resources
                    tempBuffer = Nothing
                End If
                compressedDim = BufferSize
                Exit Sub
            Else
                Throw New ZLibCompressException
            End If

        End Sub

        ''' <summary>
        ''' Decompresses the specified bytes array
        ''' </summary>
        ''' <param name="Data"></param>
        ''' <param name="Origsize"></param>
        ''' <param name="TempBuffer"></param>
        ''' <remarks></remarks>
        Public Shared Sub DeCompressBytes(ByRef data() As Byte, ByVal origSize As Integer, Optional ByRef tempBuffer() As Byte = Nothing)

            'Decompresses data from a temporary buffer. Origsize must be the size of
            'data before compression.
            'Returns data in Data if TempBuff is not specified.

            'Allocates some memory for the buffer
            Dim result As Integer
            Dim useNewStorage As Boolean
            Dim Buffersize As Integer = CInt(origSize + (origSize * 0.01) + 12)

            'Create a tempBuffer if not specified
            If tempBuffer Is Nothing Then usenewstorage = False Else usenewstorage = True
            ReDim tempBuffer(Buffersize)

            'Decompresses data
            result = NativeMethods.UncompressByteArray(tempBuffer, origSize, data, UBound(data) + 1)

            'Truncates the buffer given the compressed size
            If result = 0 Then
                If usenewstorage Then
                    'Returns compressed data in TempBuffer
                    ReDim Preserve tempBuffer(origSize - 1)
                Else
                    'Returns decompressed data to the starting array
                    'which is limited to compressed data's size
                    ReDim data(origSize - 1)
                    Array.Copy(tempBuffer, data, origSize)
                    'Releases resources
                    tempBuffer = Nothing
                End If
                originalDim = origSize
                Exit Sub

            Else

                Throw New ZLibDecompressException
            End If
        End Sub

        ''' <summary>
        ''' Compresses the specified file. If an error occurs, a ZLibCompressException is thrown
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="target"></param>
        ''' <remarks></remarks>
        Public Shared Sub Compress(ByVal source As String, ByVal target As String)

            Dim data() As Byte
            Dim size As Integer

            Try
                ReDim data(FileLen(source) - 1)

                size = UBound(data) + 1

                Dim F As New IO.FileStream(source, IO.FileMode.Open, IO.FileAccess.Read)

                data = My.Computer.FileSystem.ReadAllBytes(source)

                F.Close()
                F.Dispose()

                CompressBytes(data)

                Dim Fw As New IO.FileStream(target, IO.FileMode.Create, IO.FileAccess.Write)

                Dim Bs As New IO.BinaryWriter(Fw)

                With Bs
                    .Write(size)
                    .Write(data, 0, data.Length)
                    .Close()
                End With

                Fw.Close()

            Catch ex As IO.FileNotFoundException
                Throw
            Catch ex As ZLibCompressException
                Throw
            Catch ex As Exception
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Decompresses the specified file. If an error occurs, a ZLibDecompressException is thrown
        ''' </summary>
        ''' <param name="compressedFile"></param>
        ''' <param name="decompressedFile"></param>
        ''' <remarks></remarks>
        Public Shared Sub Decompress(ByVal compressedFile As String, ByVal decompressedFile As String)

            Dim original As Integer, data() As Byte

            Try
                Dim FR As New IO.FileStream(compressedFile, IO.FileMode.Open, IO.FileAccess.Read)

                Dim BS As New IO.BinaryReader(FR)

                original = BS.ReadInt32

                ReDim data(original)

                BS.Read(data, 0, data.Length)

                BS.Close()
                FR.Close()

                DeCompressBytes(data, original)

                Dim FW As New IO.FileStream(decompressedFile, IO.FileMode.Create, IO.FileAccess.Write)

                With FW
                    .Write(data, 0, data.Length)

                    .Close()
                End With
                Exit Sub


            Catch ex As IO.FileNotFoundException
                Throw
            Catch ex As ZLibDecompressException
                Throw
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Private Sub New()

        End Sub
    End Class



    ''' <summary>
    ''' Throws an exception if any error occurs while compressing data
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> Public Class ZLibCompressException
        Inherits Exception

        Public Sub New()
        End Sub

        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(ByVal message As String, ByVal inner As Exception)
            MyBase.New(message, inner)
        End Sub

        Protected Sub New(ByVal info As Runtime.Serialization.SerializationInfo, ByVal context As Runtime.Serialization.StreamingContext)
            MyBase.New(info, context)
        End Sub
    End Class

    ''' <summary>
    ''' Throws an exception if any error occurs while decompressing data
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> Public Class ZLibDecompressException
        Inherits Exception

        Public Sub New()
        End Sub

        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(ByVal message As String, ByVal inner As Exception)
            MyBase.New(message, inner)
        End Sub

        Protected Sub New(ByVal info As Runtime.Serialization.SerializationInfo, ByVal context As Runtime.Serialization.StreamingContext)
            MyBase.New(info, context)
        End Sub
    End Class

End Namespace
