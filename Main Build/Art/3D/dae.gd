extends MeshInstance3D


# Declare member variables here. Examples:
var mdt = MeshDataTool.new()
var meshDuplicate = mesh.duplicate()
var rng = RandomNumberGenerator.new()

var file
var uvAnimationFile = 'res://Art/3D/uvAnimation.txt'

var uvAnimation
var uvX = []

var elapsedTime = 0

# Called when the node enters the scene tree for the first time.
func _ready():
	"""rng.randomize()
	var matArray = []
	for s in range( self.get_surface_override_material_count() ):
		matArray.push_back( self.get_surface_override_material(s) )
	for s in range( self.get_surface_override_material_count() ):
		mdt.create_from_surface(mesh, 0)
		print(mdt.get_vertex_count())
		for i in range( mdt.get_vertex_count() ):
			var vertex = mdt.get_vertex(i)
			vertex += Vector3( rng.randf()*0.1, rng.randf()*0.1, rng.randf()*0.1 )
			mdt.set_vertex(i, vertex)
		mesh.surface_remove(0)
		mdt.commit_to_surface(mesh)
		self.set_surface_override_material( self.get_surface_override_material_count()-1, matArray[s] )"""
		
	file = FileAccess.open(uvAnimationFile, FileAccess.READ)
	
	var json = JSON.new()
	var error = json.parse( file.get_as_text() )
	if error == OK:
		uvAnimation = json.data
	else:
		return
	
	var startIndex = 0
	var vCount = 0
	for s in range( self.get_surface_override_material_count() ):
		mdt.create_from_surface(mesh, s)
		startIndex += vCount
		vCount = mdt.get_vertex_count()
		
		for i in range( mdt.get_vertex_count() ):
			var uv = mdt.get_vertex_uv(i)
			if( uv.x >= 100 ):
				var animationIndex = uv.x-100
				var uvIndex = floor(abs(uv.y-1) / 100)
				var custom = (abs(uv.y-1) - uvIndex*100)
				uvIndex -= 1
				
				#print(uvIndex,'  ',custom)
				
				var action = uvAnimation[ animationIndex ]
				var duration = 0
				for frame in action.frame:
					duration += frame.duration
					
				uvX.push_back( { 
					'vertexIndex': startIndex+i, 
					'animationIndex': animationIndex, 
					'uvIndex': uvIndex, 
					'custom': custom, 
					'currentFrame': 0,
					'duration': duration
				} )
	

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	elapsedTime = Time.get_ticks_msec() / 1000.0
	var updateUVs = false
	
	
	for vertex in uvX:
		var action = uvAnimation[ vertex.animationIndex ]
		var startTime = action.custom[ vertex.custom ]
		var f = 0
		var actionTime = fmod( elapsedTime + startTime, vertex.duration )
		
		var duration = 0
		for frame in action.frame:
			duration += frame.duration
			if( duration > actionTime ): break
			f += 1
			
		if( vertex.currentFrame != f ):
			vertex.currentFrame = f
			updateUVs = true
	
	
	if( updateUVs ):
		var matArray = []
		var startIndex = 0
		var vCount = 0
		
		for s in range( self.get_surface_override_material_count() ):
			matArray.push_back( self.get_surface_override_material(s) )
		
		# since 4.0 doesn't use surface_remove, we have to clear all surfaces, and use a duplicatedMesh to refer to them instead
		var surfaceOverrideCount = self.get_surface_override_material_count()
		mesh.clear_surfaces()
		
		for s in range( surfaceOverrideCount ):
			#mdt.create_from_surface(mesh, 0)
			mdt.create_from_surface(meshDuplicate, s)
			startIndex += vCount
			vCount = mdt.get_vertex_count()
			
			for vertex in uvX:
				if( vertex.vertexIndex >= startIndex && vertex.vertexIndex < startIndex+vCount ):
					var v = vertex.vertexIndex - startIndex
					var f = vertex.currentFrame
					var uv = Vector2(
						uvAnimation[ vertex.animationIndex ].frame[ f ].uv[ vertex.uvIndex ].x,
						1-uvAnimation[ vertex.animationIndex ].frame[ f ].uv[ vertex.uvIndex ].y
						)
					mdt.set_vertex_uv( v, uv )
			
			#mesh.surface_remove(0)	# this was available in 3.5, but isn't in 4.0 (it is planned to be added in 4.x)
			mdt.commit_to_surface(mesh)
			#self.set_surface_override_material( self.get_surface_override_material_count()-1, matArray[s] )
			self.set_surface_override_material( s, matArray[s] )
