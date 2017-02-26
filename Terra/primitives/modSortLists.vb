Module modSortLists
	Public Sub sort_models_by_texID()
		Dim current_texID As Integer = 1
		ReDim model_batchList(1)
		model_batchList(0) = New m_batch_
		ReDim model_batchList(0).model(1)
		model_batchList(0).model(0) = New m_model_
		model_batchList(0).count = 0
		Dim max_texture_id As Integer = 0
		Dim min_texture_id As Integer = 10000
		For map = 0 To test_count
			For j = 0 To maplist(map).models.Length - 1
				'First, we need to know where the bounds are for the texIDs so we dont waste time testing for non-existant textures
				For k = 0 To maplist(map).models(j)._count - 1
					If Gl.glIsList(maplist(map).models(j).componets(k).callList_ID) Then
						If maplist(map).models(j).componets(k).color_id > max_texture_id Then
							max_texture_id = maplist(map).models(j).componets(k).color_id
						End If
						If maplist(map).models(j).componets(k).color_id < min_texture_id Then
							min_texture_id = maplist(map).models(j).componets(k).color_id
							Debug.Write(min_texture_id.ToString + vbCrLf)
						End If
					End If

				Next
			Next
		Next
		Dim position As Integer = 0
		For textID = min_texture_id To max_texture_id
			ReDim Preserve model_batchList(position + 1)
			model_batchList(position) = New m_batch_
			model_batchList(position).texture_ID = textID
			For map = 0 To test_count
				For j = 0 To maplist(map).models.Length - 1

					For k = 0 To maplist(map).models(j)._count - 1
						If Gl.glIsList(maplist(map).models(j).componets(k).callList_ID) Then
							If textID = maplist(map).models(j).componets(k).color_id Then
								model_batchList(position).count += 1
								ReDim Preserve model_batchList(position).model(model_batchList(position).count)
								Dim count = model_batchList(position).count - 1
								model_batchList(position).model(count).texture2_ID = maplist(map).models(j).componets(k).color2_Id
								model_batchList(position).model(count).norm_ID = maplist(map).models(j).componets(k).normal_Id

								model_batchList(position).model(count + 1) = New m_model_

								If maplist(map).models(j).componets(k).GAmap Then
									model_batchList(position).model(count).GAmap = 1
								Else
									model_batchList(position).model(count).GAmap = 0
								End If

								If maplist(map).models(j).componets(k).multi_textured Then
									model_batchList(position).model(count).multi_textured = 1
								Else
									model_batchList(position).model(count).multi_textured = 0
								End If

								If maplist(map).models(j).componets(k).bumped Then
									model_batchList(position).model(count).bumped = 1
								Else
									model_batchList(position).model(count).bumped = 0
								End If
								model_batchList(position).model(count).alphaRef =
									maplist(map).models(j).componets(k).alphaRef

								model_batchList(position).model(count).alphaTestEnable =
									maplist(map).models(j).componets(k).alphaTestEnable

								model_batchList(position).model(count).transform = New transformStruct
								Dim t_trans = New transformStruct
								t_trans = maplist(map).model_transforms(j)
								t_trans.matrix = maplist(map).model_transforms(j).matrix

								model_batchList(position).model(count).transform.matrix = t_trans.matrix

								model_batchList(position).model(count).transform = t_trans

								model_batchList(position).model(count).display_list_id = maplist(map).models(j).componets(k).callList_ID
								Dim t_vect As vect3 = maplist(map).location
								model_batchList(position).model(count).location = t_vect
							End If
						End If

					Next
				Next
			Next
			position += 1
		Next textID

	End Sub
End Module
