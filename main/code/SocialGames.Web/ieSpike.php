<!DOCTYPE html>
<html lang="en">
	<head>
        <meta charset="utf-8">
        
        <meta property="og:image" content="images/socialMedia/tanksters-icon-50by50.png"/>
        
        <meta property="og:title" content="Tankster"/>
        <meta property="og:type" content="game"/>
        <meta property="og:url" content="http://www.tankster.net"/>
        <meta property="og:site_name" content="Tankster"/>
        <meta property="fb:app_id" content="246416648720668"/>
        <meta property="og:description" content="War has erupted on the beach! Dominate other players using a range of weapons and defenses that demolish the players, as well as the surrounding environment. If you can't claim a spot in the leader board as the best tank in the game, humiliate your opponent by giving them an embarrassing decal or tank design, or hone your craft in practice battles."/>
        
        <meta http-equiv="X-UA-Compatible" content="IE=Edge">
        
		<title>Tankster</title>
        
        <link rel="shortcut icon" type="image/x-icon" href="images/favicon.ico">

        <link type="text/css" href="http://yui.yahooapis.com/2.9.0/build/reset/reset-min.css" rel="stylesheet" />
        <link type="text/css" href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.13/themes/humanity/jquery-ui.css" rel="stylesheet" />
        <link type="text/css" href="css/devUI.css?no=<? echo(uniqid()); ?>" rel="stylesheet" />
        
        <script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jquery/jquery-1.6.2.min.js"></script>
		<script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.13/jquery-ui.min.js"></script>
        
        <script type="text/javascript" src="lib/flXHR-1.0.6/flXHR.js"></script>
	    <script type="text/javascript" src="lib/flXHR-1.0.6/jquery.flXHRproxy.js"></script>

        <script type="text/javascript" src="lib/jQueryColor.js"></script>
        <script type="text/javascript" src="lib/json2.js"></script>
        <script type="text/javascript" src="lib/easel.js"></script>
        <script type="text/javascript" src="lib/SpriteSheetUtils2.js"></script>

        

        
        
        <?php
            $noCache = array();
            $noCache[] = 'lib/SoundJS.js';
            $noCache[] = 'lib/Tween.js';

            $noCache[] = 'src/ui/collections/ArrayList.js';
            
            $noCache[] = 'src/game/util/Rndm.js';
            $noCache[] = 'src/com/gskinner/events/EventDispatcher.js';
            $noCache[] = 'src/com/gskinner/utils/*';
            $noCache[] = 'src/net/ServerRequest.js';
            $noCache[] = 'src/net/ServerDelegate.js';
            $noCache[] = 'src/net/*';
            $noCache[] = 'src/model/Strings.js';   
                    
            $noCache[] = 'src/model/*';
        
            $noCache[] = 'src/ui/renderers/WeaponTile.js';
            $noCache[] = 'src/ui/renderers/PowerTile.js';
            $noCache[] = 'src/ui/renderers/PrankTile.js';
            $noCache[] = 'src/ui/renderers/LeaderboardRenderer.js';
            $noCache[] = 'src/ui/renderers/TabBarRenderer.js';
            $noCache[] = 'src/ui/renderers/StoreRenderer.js';
            $noCache[] = 'src/ui/renderers/InventoryRenderer.js';

            $noCache[] = 'src/ui/controls/*';

            $noCache[] = 'src/ui/managers/ViewManager.js';
        
            $noCache[] = 'src/ui/managers/PopupManager.js';
            $noCache[] = 'src/ui/managers/ToolTipManager.js';

            $noCache[] = 'src/ui/dialogs/*';
            $noCache[] = 'src/ui/views/*';
            $noCache[] = 'src/ui/controller/CommandMap.js';

            $noCache[] = 'src/game/Game.js';
            $noCache[] = 'src/game/GameManager.js';
			
            $noCache[] = 'src/game/actors/Actor.js';
            $noCache[] = 'src/game/actors/Tank.js';
            $noCache[] = 'src/ui/commands/*';
            $noCache[] = 'src/game/actors/*';
            $noCache[] = 'src/game/weapons/WeaponUtil.js';
            $noCache[] = 'src/game/weapons/*';
            $noCache[] = 'src/game/env/*';

            //Build list of scripts.
            $paths = array();
            $l = count($noCache);
            for ($i=0;$i<$l;$i++) {
                $path = $noCache[$i];
                if (strrpos($path, '*') !== false) {
                    $basePath = substr($path, 0, strlen($path)-1);
                    $files = scandir($basePath);
                    foreach ($files as $key => $value) {
                        if (substr($value, 0, 1) == '.') {
                            continue;
                        }

                        if (!is_dir($value) !== false && array_pop(explode('.', $value)) == 'js') {
                            $paths[] = $basePath.$value;
                        }
                    }
                } else {
                    $paths[] = $path;
                }
            }

            //Write the script tags
            $tagHash = array();
            foreach ($paths as $key => $value) {
                if (key_exists($value, $tagHash)) {
                    continue;
                }
                
                $tagHash[$value] = true;
                echo(sprintf("\t\t".'<script type="text/javascript" src="%s"></script>'."\n", $value ));//. '?noCache=' . mt_rand()));
            }
        ?>

        <script>
            function init(){

                var requiredImages = [];
                //requiredImages.push({name:'gameChromeCharacters', url:'images/common/characters/characters-SpriteSheet.png'});
                requiredImages.push({name:'gameChromeCharacters', url:'images/common/characters/flipchar.png'});
                requiredImages.push({name:'gameChromeWeapons', url:'images/game/weapons/weaponSelection-powerUpIcons.png'});
                //requiredImages.push({name:'gameChromeCharacters', url:'images/game/weapons/weaponSelection-powerUpIcons2.png'});
                requiredImages.push({name:'tank_generic_base', url:'images/game/tanks/generic-TankColor.png'});
                requiredImages.push({name:'tank_generic_detail', url:'images/game/tanks/generic-TankColorFixed.png'});
                requiredImages.push({name:'turret_generic_base', url:'images/game/tanks/generic-TurretColor.png'});
                requiredImages.push({name:'turret_generic_detail', url:'images/game/tanks/generic-TurretColorFixed.png'});
                requiredImages.push({name:'targeter_ingame', url:'images/game/icon-target.png'});
                requiredImages.push({name:'parachute', url:'images/game/powerup-parachute.png'});
                requiredImages.push({name:'dirtball_lrg', url:'images/game/weapons/pjctl-bomb-dirtball-large.png'});
                requiredImages.push({name:'dirtball_sml', url:'images/game/weapons/pjctl-bomb-dirtball-small.png'});
                requiredImages.push({name:'excavator_lrg', url:'images/game/weapons/pjctl-bomb-excavator-large.png'});
                requiredImages.push({name:'excavator_sml', url:'images/game/weapons/pjctl-bomb-excavator-small.png'});
                requiredImages.push({name:'napalm_lrg', url:'images/game/weapons/pjctl-bomb-napalm-large.png'});
                requiredImages.push({name:'napalm_sml', url:'images/game/weapons/pjctl-bomb-napalm-small.png'});
                requiredImages.push({name:'roller_lrg', url:'images/game/weapons/pjctl-bomb-roller-large.png'});
                requiredImages.push({name:'roller_sml', url:'images/game/weapons/pjctl-bomb-roller-small.png'});
                requiredImages.push({name:'shotgun', url:'images/game/weapons/pjctl-bullet-shotgun.png'});
                requiredImages.push({name:'sniper', url:'images/game/weapons/pjctl-bullet-sniper.png'});
                requiredImages.push({name:'nuclear', url:'images/game/weapons/projectile-missile-nuke.png'});
                requiredImages.push({name:'deathhead', url:'images/game/weapons/pjctl-missile-deathshead.png'});
                requiredImages.push({name:'bunkerbuster', url:'images/game/weapons/pjctl-missile-bunkerbuster.png'});
                requiredImages.push({name:'missle_lrg', url:'images/game/weapons/pjctl-missile-large.png'});
                requiredImages.push({name:'missle_med', url:'images/game/weapons/pjctl-missile-medium.png'});
                requiredImages.push({name:'missle_sml', url:'images/game/weapons/pjctl-missile-small.png'});

                ImageLoader.load(requiredImages, null, handleImagesLoad);
            }

            function handleImagesLoad(){
                var weaponFrameData = {
                    missileTriple:[0,0],
                    sniper:[1,1],
                    napalmSmall:[2,2],
                    missileSmall:[3,3],
                    dirtBombSmall:[4,4],
                    shotgun:[5,5],
                    rollerSmall:[6,6],
                    missileNuke:[7,7],
                    missileMedium:[8,8],
                    rollerLarge:[9,9],
                    napalmLarge:[10,10],
                    missileLarge:[11,11],
                    excavatorLarge:[12,12],
                    dirtBombLarge:[13,13],
                    excavator:[14,14],
                    deathsHead:[15,15],
                    napalmCluster:[16,16],
                    missileCluster:[17,17],
                    dirtBombCluster:[18,18],
                    bunkerBuster:[19,19]
                };
                var flipWeaponFrameData = {};
                for (var n in weaponFrameData) {
                    flipWeaponFrameData[n+'_right'] = [n, true];
                }
                var weaponSpriteSheet = new SpriteSheet(ImageLoader.getImage('gameChromeWeapons'),128,128,weaponFrameData);
                weaponSpriteSheet = SpriteSheetUtils2.flip(weaponSpriteSheet, flipWeaponFrameData);

                //wdg:: To fix a bug in easel, we need to re-create the canvas.
                SpriteSheetUtils2._workingCanvas = document.createElement("canvas");
                SpriteSheetUtils2._workingContext = SpriteSheetUtils2._workingCanvas.getContext("2d");

                playerOneWeaponsDiv = $('#playerOneWeapon')[0];
                playerTwoWeaponsDiv = $('#playerTwoWeapon')[0];

                playerOneWeapon = new AnimatedImage(playerOneWeaponsDiv, weaponSpriteSheet, AnimatedImage.LEFT);
                playerTwoWeapon = new AnimatedImage(playerTwoWeaponsDiv, weaponSpriteSheet, AnimatedImage.RIGHT);

                //Set-up the characters
                var charFrameData = {
                    muscles:[0,0],
                    general:[1,1],
                    girl:[2,2],
                    veteran:[3,3],
                    grunt:[4,4],

                    muscles_right:[5,5],
                    general_right:[6,6],
                    girl_right:[7,7],
                    veteran_right:[8,8],
                    grunt_right:[9,9]                  
                };
                var flipCharFrameData = {};
                for (var n in charFrameData) {
                    flipCharFrameData[n+'_right'] = [n, true];
                }
                var characterSpriteSheet = new SpriteSheet(ImageLoader.getImage('gameChromeCharacters'),123,128,charFrameData);
                //characterSpriteSheet = SpriteSheetUtils2.flip(characterSpriteSheet, flipCharFrameData);

                SpriteSheetUtils2._workingCanvas = document.createElement("canvas");
                SpriteSheetUtils2._workingContext = SpriteSheetUtils2._workingCanvas.getContext("2d");

                playerOneAvatarDiv = $('#playerOneAvatar')[0];
                playerTwoAvatarDiv = $('#playerTwoAvatar')[0];

                playerOneAvatar = new AnimatedImage(playerOneAvatarDiv, characterSpriteSheet, AnimatedImage.LEFT);
                playerTwoAvatar = new AnimatedImage(playerTwoAvatarDiv, characterSpriteSheet, AnimatedImage.RIGHT);

                playerOneAvatar.show(CharacterSprites.frameOrder[UserModel.getCharacter()], playerOneWeapon, false);
                playerOneWeapon.show('rollerLarge', null, false);
                playerOneAvatar.play();
                //playerOneAvatar.show('grunt', null, false);
                //playerOneAvatar.play();
                //playerOneWeapon.show('rollerLarge', null, false);
                //playerOneWeapon.play();
            
                playerTwoAvatar.show('general_right', playerTwoWeapon, false);
                playerTwoWeapon.show('missileNuke_right', null, false);
                playerTwoAvatar.play();
                //playerTwoAvatar.show('girl_right', null, false);
                //playerTwoAvatar.play();
                //playerTwoWeapon.show('missileNuke_right', null, false);
                //playerTwoWeapon.play();
            }
            $(init);
        </script>
        
	</head>
    
	<body>

        <div  id="playerOneDiv">
            <div id="playerOneAvatar"></div>
            <div id="playerOneWeapon"></div>
            <div id="playerOneNamePlate" class="bg-3">
            	<div class="player-namePlateWrap">
                    <p id="playerOneUsername">Player 1</p>
                    <div id="userOneChangeWeaponButton" class="button button-3 buttonWidth-160"><div>{CHANGE_WEAPON_BTN}</div></div>
                </div>
            </div>
        </div>

        <div  id="playerTwoDiv">
            <div id="playerTwoAvatar"></div>
            <div id="playerTwoWeapon"></div>
            <div id="playerTwoNamePlate" class="bg-1">
            	<div class="player-namePlateWrap">
                    <p id="playerTwoUsername">Player 2</p>
                    <div id="userTwoChangeWeaponButton" class="button button-1 buttonWidth-160"><div>{CHANGE_WEAPON_BTN}</div></div>
            	</div>
            </div>
        </div>

	</body>
</html>
