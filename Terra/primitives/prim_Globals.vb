Module prim_Globals


    Public uv2() As vect2uv
    Public uv2_b() As vect2uv


    Public ind_scale As UInt32 = 0
    Public stride As UInt32 = 0

    Public Structure indi
        Public a1 As UInt32
        Public a2 As UInt32
        Public a3 As UInt32

    End Structure
    Public Structure primitive
        Public _count As Integer
        Public componets() As Model_Section
        Public isBuilding As Boolean 'Used to render buildings only on first pass. Its decal rendering logic.
        Public visible As Boolean
    End Structure
    Public Structure Model_Section
        Public callList_ID As Int32
        ' Public indices() As Integer
        Public vertices() As vect3
		Public normals() As vect3Norm
		Public tangents() As vect3Norm
        Public binormals() As vect3Norm
		Public UVs() As vect2uv
        Public UV2s() As vect2uv
        Public name As String
        'for texture mapping
        Public color_id As Int32
        Public normal_Id As Int32
		Public color2_Id As Integer
        Public _count As UInt32
        Public multi_textured As Boolean
        Public alpha_only As Boolean
		Public bumped As Boolean
		Public GAmap As Boolean
		Public color2_name As String
		Public color_name As String
        Public normal_name As String
		Public alphaRef As Integer
		Public alphaTestEnable As Integer
    End Structure
    Public Structure section_info
        Public name As String
        Public location As UInt32
        Public size As UInt32
    End Structure
    Structure primGroup
        Public startIndex_ As Long
        Public nPrimitives_ As Long
        Public startVertex_ As Long
        Public nVertices_ As Long
    End Structure
    Structure IndexHeader
        Public ind_h As String
        Public nIndices_ As UInt32
        Public nInd_groups As UShort
    End Structure
    Structure VerticesHeader
        Public header_text As String
        Public nVertice_count As UInt32
    End Structure
    Structure VertexXYZNUVTB
        Public x As Single
        Public y As Single
        Public z As Single
        Public normal_ As UInt32
        Public uv_ As vect2
        Public tangent_ As UInt32
        Public binormal_ As UInt32
    End Structure
    Structure VertexZYZNUVIIIWWTB
        Public x As Single
        Public y As Single
        Public z As Single
        Public normal_ As UInt32
        Public uv_ As vect2
        Public index1 As Byte
        Public index2 As Byte
        Public index3 As Byte
        Public weight1 As Byte
        Public weight2 As Byte
        Public tangent_ As UInt32
        Public binormal_ As UInt32

    End Structure
    Structure bsp_header

        Public magic As UInt32
        Public numTriangles As Int32
        Public numNodes As Int32
        Public maxTriangles As Int32
    End Structure

End Module
