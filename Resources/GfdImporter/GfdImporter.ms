
/* 	Persona 3: DMN, Persona 4: DAN, Persona5: DSN & Persona 5 model and animation importer (*.gmd; *.gfs; *.gap) written in MaxScript
    Version 0.14
    Script by TGE. Please give credit where is due.
    Special thanks to PolarSoda, mrdudesworkshop
    
    Changelog:
    v0.14:
        Implement support for importing morphers.
        Implement partial incomplete support for loading morph animations.
    
    v0.13:
        Further improve support for P3D/P5D models.
        Add checkbox to control whether or not to reset animation before loading a new one.
        Automatically enable backface culling for imported models.
        Support new animation keyframe types used in P3D/P5D animations.
    
    v0.12:
        Improve support for P3D/P5D models.
    
    v0.11:
        Reset button now properly resets the skeleton to its initial default pose.
        Note that this uses the pose the model is in when you first load an animation!
    
    v0.10:
        Implemented a GUI for configuring options, and selecting which animation to load
            - There's 2 types of animations: Normal, and Blend animations.
              Blend animations are overlaid ontop of other animations
        Complete rewritten animation file parsing based on my latest research
        Various code cleanups
        Additions to brute force method for particle attachments, making more models load
        Fixed a bug that caused some models to not load (eg. Arsene)
        
    v0.9:
        Proper parsing of materials, morphers, meshes, lights, cameras; in short: everything works a bit better now
        Implemented brute force parsing for particle attachments, makes quite a few enemies load now
        Removed redudant "RootNode" node
        Improved naming of things
        Disabled 'ghosting' of the process while the script is running which prevents 3ds max from locking up, hope it works
        Implemented parsing of additional property types
        Misc bug fixes
*/

FileIn "include\\MaxScriptExtensions\\DisableProcessWindowsGhosting.ms"
FileIn "include\\MaxScriptExtensions\\FileUtils.ms"
FileIn "include\\GfdFileLoader.ms"

(
    local loader = GfdFileLoader()
    local importedModel = undefined
    local transforms = undefined
    
    fn ResetAnimation =
    (
        for i = 1 to objects.Count do DeleteKeys objects[i].Controller #allkeys
        for i = 1 to objects.Count do
        (
            local obj = objects[i];
            
            obj.Controller.Position = transforms[i][1]
            obj.Controller.Rotation = transforms[i][2]
            obj.Controller.Scale = transforms[i][3]
            
            local hasMorpherMod = false;
            for m in obj.modifiers do
            (
                if ( classof m == morpher ) then
                (
                    hasMorpherMod = true;
                    break;
                )
            )
            
            if ( hasMorpherMod ) then
            (
                for i = 1 to 100 do
                (
                    if ( WM3_MC_HasData obj.Morpher i ) then
                        WM3_MC_SetValue obj.Morpher i 0.0;
                )
            )
        )
    )

    rollout GfdImporterRollout "GMD/GFS/GAP importer" width:350 height:360
    (
    	label 'versionLabel' "GMD/GFS/GAP importer version 0.14 Alpha by TGE" pos:[50,15] width:250 height:25 align:#left
    	GroupBox 'optionsGroupBox' "Options" pos:[20,50] width:306 height:111 align:#left
    	checkbox 'applyNormalsCheckbox' "Apply Normals" pos:[35,70] width:96 height:15 enabled:false checked:true align:#left
    	checkbox 'applySkinCheckbox' "Apply Skin" pos:[35,110] width:96 height:15 enabled:false checked:true align:#left
    	checkbox 'applyColorsCheckbox' "Apply Colors" pos:[35,90] width:96 height:15 enabled:false checked:true align:#left
    	checkbox 'yToZUpCheckbox' "Convert Y to Z up" pos:[35,130] width:103 height:15 enabled:false checked:false align:#left
    	checkbox 'overwriteTexturesCheckbox' "Overwrite Existing Textures" pos:[145,70] width:154 height:15 enabled:false checked:false align:#left
    	checkbox 'exportTexturesOnlyCheckbox' "Export Textures only" pos:[145,90] width:165 height:15 checked:false align:#left
    	checkbox 'resetAnimOnLoad' "Reset Animation On Load" pos:[145,110] width:165 height:15 enabled:false checked:true align:#left
    	button 'importFileButton' "Import File" pos:[125,278] width:100 height:30 align:#left   
   
    	button 'loadModelButton' "Load Model" pos:[15,313] width:100 height:30 enabled:false align:#left
    	button 'loadAnimationButton' "Load Animation" pos:[125,313] width:100 height:30 enabled:false align:#left
    	spinner 'animationIndexSpinner' "" pos:[190,243] width:70 height:16 enabled:false type:#integer align:#left
    	label 'animationIndexLabel' "Animation Index" pos:[85,243] width:80 height:20 align:#left
    	label 'animationsLoadedLabel' "Animations Loaded:" pos:[85,223] width:93 height:14 align:#left
    	label 'animationsLoadedNumberLabel' "0" pos:[196,223] width:55 height:13 align:#left   
   
    	dropdownList 'animationTypeDropdown' "Animation Type" pos:[134,170] width:82 height:40 enabled:false items:#("Normal", "Blend") selection:1 align:#left
    	button 'resetAnimationButton' "Reset Animation" pos:[235,313] width:100 height:30 enabled:false align:#left

    	on importFileButton pressed do
    	(
    	    GC()
    	    GC()
    	    ClearListener()
    	    
    	    local filePath = FileUtils.RunOpenFileDialog "Open model or animation file" "Persona 5 Model (*.gmd; *.gfs; *.gap)|*.gmd; *.gfs; *.gap" "P5 Object Presets"
    	    if ( filePath != undefined ) then
    	    (
    	        if (not (GfdFileLoader.IsValidFile( filePath ))) then
    	        (
    	            Format "Not a valid Persona 5 model file. (File doesn't start with GFS0 signature)\n"
    	        )
    	        else
    	        (
    	            -- Configure loader options
    	            loader.ApplyNormals = applyNormalsCheckbox.Checked
    	            loader.ApplyColors = applyColorsCheckbox.Checked
    	            loader.ApplySkin = applySkinCheckbox.Checked
    	            loader.ApplyYToZUp = yToZUpCheckbox.Checked
    	            loader.OverwriteExistingTextures = overwriteTexturesCheckbox.Checked
    	            loader.ExportTexturesOnly = exportTexturesOnlyCheckbox.Checked
    	            
    	            -- Load model
    	            try (	
    	                
    	                with redraw off
    	                (
    	                    importedModel = loader.Load( filepath )
    	                )
    	                
    	                Format "File loaded successfully.\n"
    	                
    	            ) catch (Print (getCurrentException()))
    	               
    	            -- Update UI
    	            if ( importedModel != undefined ) then
    	            (
    	                if ( importedModel.Scene != undefined ) then
    	                (
    	                    applyNormalsCheckbox.Enabled = true
    	                    applySkinCheckbox.Enabled = true
    	                    applyColorsCheckbox.Enabled = true
    	                    yToZUpCheckbox.Enabled = true
    	                    loadModelButton.Enabled = true
    	                    
    	                    transforms = #()
                            for i = 1 to objects.Count do 
                                append transforms #( objects[i].Controller.Position, objects[i].Controller.Rotation, objects[i].Controller.Scale )
    	                    
    	                    resetAnimationButton.Enabled = true
    	                )
    	                else
    	                (
    	                    applyNormalsCheckbox.Enabled = false
    	                    applySkinCheckbox.Enabled = false
    	                    applyColorsCheckbox.Enabled = false
    	                    yToZUpCheckbox.Enabled = false
    	                    loadModelButton.Enabled = false
    	                    resetAnimationButton.Enabled = true;
                            
                            if ( transforms == undefined ) then
                            (
                                transforms = #()
                                for i = 1 to objects.Count do 
                                    append transforms #( objects[i].Controller.Position, objects[i].Controller.Rotation, objects[i].Controller.Scale )
                            )
    	                )
    	                
    	                loadAnimationButton.Enabled = false
    	                animationTypeDropdown.Enabled = false
    	                animationsLoadedNumberLabel.Text = "0"
    	                animationTypeDropdown.Selection = 1
    	                
    	                if ( importedModel.AnimationPackage != undefined ) then
    	                (
    	                    local animationCount = importedModel.AnimationPackage.AnimationCount
    	                    if ( animationCount == 0 ) then
    	                    (
    	                        animationCount = importedModel.AnimationPackage.BlendAnimationCount
    	                        animationTypeDropdown.Selection = 2
    	                    )
    	                    
    	                    animationsLoadedNumberLabel.Text = animationCount as string
    	                    animationTypeDropdown.Enabled = true
    	                    
    	                    if ( animationCount != 0 ) then
    	                    (
    	                        animationIndexSpinner.Range = [0, animationCount - 1, 0]              
    	                        animationIndexSpinner.Enabled = true
    	                        loadAnimationButton.Enabled = true
                                resetAnimOnLoad.Enabled = true;
    	                    )
    	                    else
    	                    (
    	                        animationTypeDropdown.Selection = 1
    	                        animationIndexSpinner.Range = [0, 0, 0]   
    	                        animationIndexSpinner.Enabled = false
    	                        loadAnimationButton.Enabled = false
                                resetAnimOnLoad.Enabled = false;
    	                    )
    	                )
    	            )
    	            else
    	            (
    	                applyNormalsCheckbox.Enabled = false
    	                applySkinCheckbox.Enabled = false
    	                applyColorsCheckbox.Enabled = false
    	                yToZUpCheckbox.Enabled = false
    	                loadModelButton.Enabled = false
    	                loadAnimationButton.Enabled = false
    	                animationsLoadedNumberLabel.Text = "0"
    	                animationIndexSpinner.Enabled = false
    	                animationTypeDropdown.Selection = 1
    	                animationTypeDropdown.Enabled = false
    	            )
    	            
    	            -- Clean up, not that it will help much
    	            GC()
    	            GC()
    	        )
    	    )
    	)
    	on loadModelButton pressed do
    	(
    	    -- Configure loader options
    	    loader.ApplyNormals = applyNormalsCheckbox.Checked
    	    loader.ApplyColors = applyColorsCheckbox.Checked
    	    loader.ApplySkin = applySkinCheckbox.Checked
    	    loader.ApplyYToZUp = yToZUpCheckbox.Checked
    	    loader.OverwriteExistingTextures = overwriteTexturesCheckbox.Checked
    	    loader.ExportTexturesOnly = exportTexturesOnlyCheckbox.Checked
    	    
    	    loader.SetupScene( importedModel )
    	    transforms = undefined
    	)
    	on loadAnimationButton pressed do
    	(
    	    if ( transforms == undefined ) then
    	    (
    	        -- Save default pose
    	        transforms = #()
    	        for i = 1 to objects.Count do 
    	            append transforms #( objects[i].Controller.Position, objects[i].Controller.Rotation, objects[i].Controller.Scale )
    	    )
    	    else if ( resetAnimOnLoad.Checked ) then
    	    (
    	        ResetAnimation()
    	    )
    	    
    	    if ( importedModel.AnimationPackage != undefined ) then
    	    (
    	        local animation = undefined
    	        local isBlend = false
    	        if ( animationTypeDropdown.Selection == 1 ) then
    	        (
    	            animation = importedModel.AnimationPackage.Animations[ animationIndexSpinner.value + 1 ]
    	        )
    	        else if ( animationTypeDropdown.Selection == 2 ) then
    	        (
    	            animation = importedModel.AnimationPackage.BlendAnimations[ animationIndexSpinner.value + 1 ]
    	            isBlend = true
    	        )
    	        
    	        if ( animation != undefined ) then
    	        (
    	            loader.SetupAnimation animation isBlend
    	        )
    	    )
    	)
    	on animationTypeDropdown selected sel do
    	(
    	    if ( importedModel.AnimationPackage != undefined ) then
    	    (
    	        local animationCount = 0
    	        if ( sel == 1 ) then
    	        (
    	            animationCount = importedModel.AnimationPackage.AnimationCount
    	        )
    	        else if ( sel == 2 ) then
    	        (
    	            animationCount = importedModel.AnimationPackage.BlendAnimationCount
    	        )
    	        
    	        animationsLoadedNumberLabel.Text = animationCount as string
    	        animationTypeDropdown.Enabled = true
    	        
    	        if ( animationCount != 0 ) then
    	        (
    	            animationIndexSpinner.Range = [0, animationCount - 1, 0]              
    	            animationIndexSpinner.Enabled = true
    	            loadAnimationButton.Enabled = true
    	        )
    	        else
    	        (
    	            animationIndexSpinner.Range = [0, 0, 0]   
    	            animationIndexSpinner.Enabled = false
    	            loadAnimationButton.Enabled = false
    	            
    	        )
    	    )
    	)
    	on resetAnimationButton pressed do
    	(
    	    ResetAnimation()
    	)
    )
    
    CreateDialog GfdImporterRollout
)
