<!DOCTYPE html>
<html lang="en">
	<head>
        <meta charset="utf-8">
        
        <meta property="og:image" content="Content/images/socialMedia/tanksters-icon-50by50.png"/>
        
        <meta property="og:title" content="Tankster"/>
        <meta property="og:type" content="game"/>
        <meta property="og:url" content="http://www.tankster.net"/>
        <meta property="og:site_name" content="Tankster"/>
        <meta property="fb:app_id" content="246416648720668"/>
        <meta property="og:description" content="War has erupted on the beach! Dominate other players using a range of weapons and defenses that demolish the players, as well as the surrounding environment. If you can't claim a spot in the leader board as the best tank in the game, humiliate your opponent by giving them an embarrassing decal or tank design, or hone your craft in practice battles."/>
        
        <meta http-equiv="X-UA-Compatible" content="IE=Edge">
        
		<title>Tankster</title>
        
        <link rel="shortcut icon" type="image/x-icon" href="Content/images/favicon.ico">

        <link type="text/css" href="http://yui.yahooapis.com/2.9.0/build/reset/reset-min.css" rel="stylesheet" />
        <link type="text/css" href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.13/themes/humanity/jquery-ui.css" rel="stylesheet" />
        <link type="text/css" href="Content/css/devUI.css?no=<? echo(uniqid()); ?>" rel="stylesheet" />
        
        <script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jquery/jquery-1.6.2.min.js"></script>
		<script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.13/jquery-ui.min.js"></script>
        
        <script type="text/javascript" src="Scripts/lib/flXHR-1.0.6/flXHR.js"></script>
	    <script type="text/javascript" src="Scripts/lib/flXHR-1.0.6/jquery.flXHRproxy.js"></script>

        <script type="text/javascript" src="Scripts/lib/jQueryColor.js"></script>
        <script type="text/javascript" src="Scripts/lib/json2.js"></script>
        <script type="text/javascript" src="Scripts/lib/easel.js"></script>
        
        <?php
            $noCache = array();
            $noCache[] = 'Scripts/lib/SoundJS.js';
            $noCache[] = 'Scripts/lib/Tween.js';

            $noCache[] = 'Scripts/src/ui/collections/ArrayList.js';
            
            $noCache[] = 'Scripts/src/game/util/Rndm.js';
            $noCache[] = 'Scripts/src/com/gskinner/events/EventDispatcher.js';
            $noCache[] = 'Scripts/src/com/gskinner/utils/*';
            $noCache[] = 'Scripts/src/net/ServerRequest.js';
            $noCache[] = 'Scripts/src/net/ServerDelegate.js';
            $noCache[] = 'Scripts/src/net/*';
            $noCache[] = 'Scripts/src/model/Strings.js';   
                    
            $noCache[] = 'Scripts/src/model/*';
        
            $noCache[] = 'Scripts/src/ui/renderers/WeaponTile.js';
            $noCache[] = 'Scripts/src/ui/renderers/PowerTile.js';
            $noCache[] = 'Scripts/src/ui/renderers/PrankTile.js';
            $noCache[] = 'Scripts/src/ui/renderers/LeaderboardRenderer.js';
            $noCache[] = 'Scripts/src/ui/renderers/TabBarRenderer.js';
            $noCache[] = 'Scripts/src/ui/renderers/InventoryRenderer.js';

            $noCache[] = 'Scripts/src/ui/controls/TileList.js';
            $noCache[] = 'Scripts/src/ui/controls/*';

            $noCache[] = 'Scripts/src/ui/managers/ViewManager.js';
        
            $noCache[] = 'Scripts/src/ui/managers/PopupManager.js';
            $noCache[] = 'Scripts/src/ui/managers/ToolTipManager.js';

			$noCache[] = 'Scripts/src/game/GameManager.js';
            $noCache[] = 'Scripts/src/ui/dialogs/*';
            $noCache[] = 'Scripts/src/ui/views/*';
            $noCache[] = 'Scripts/src/ui/controller/CommandMap.js';

            $noCache[] = 'Scripts/src/game/Game.js';
            $noCache[] = 'Scripts/src/game/GameManager.js';
			
            $noCache[] = 'Scripts/src/game/actors/Actor.js';
            $noCache[] = 'Scripts/src/game/actors/Tank.js';
            $noCache[] = 'Scripts/src/ui/commands/*';
            $noCache[] = 'Scripts/src/game/actors/*';
            $noCache[] = 'Scripts/src/game/weapons/WeaponUtil.js';
            $noCache[] = 'Scripts/src/game/weapons/*';
            $noCache[] = 'Scripts/src/game/env/*';

            //$noCache[] = 'Scripts/src/Main.js';
        
        /*
         * //wdg:: Source to recursively list all js files ... needs dependency checking
            $basePaths = array();
            $basePaths[] = 'src/';

            function findPaths($parent, &$noCache) {
                $files = scandir($parent);
                foreach ($files as $key => $value) {
                    if (substr($value, 0, 1) == '.') {
                        continue;
                    }

                    if (is_dir($parent.$value) !== false) {
                        findPaths($parent.$value.'/', $noCache);
                    } else if (array_pop(explode('.', $value)) == 'js') {
                        $noCache[] = $parent.$value;
                    }
                }
            }


            $l = count($basePaths);
            for ($i=0;$i<$l;$i++) {
                findPaths($basePaths[$i], $noCache);
            }
        */

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
                echo(sprintf("\t\t".'<script type="text/javascript" src="%s"></script>'."\n", $value . '?noCache=' . mt_rand()));
            }

            echo('
                <script type="text/javascript">
                    ImageLoader.BASE_URL = "Content/";
                    SoundManager.BASE_URL = "Content/audio/";
                    ServerDelegate.BASE_URL = "Content/";
                </script>
            ');

            echo(sprintf("\t\t".'<script type="text/javascript" src="Scripts/src/Main.js?noCache='.mt_rand().'"></script>'));
        ?>
        
	</head>
    
	<body>
        
        <!-- Dev controls -->
        <div style="position: absolute; top: 0; z-index: 100000; visibility: hidden;">
            <div>
                <label for="viewList">Views:</label>
                <select id="viewList">
                </select>
                <input id="viewSelector" type="button" value="Refresh" >
            </div>

            <div>
                <label>Dialogs</label>
                <select id="dialogsList"></select>
                <input id="dialogSelector" type="button" value="Refresh">
            </div>

        </div>

        <!-- SITE WRAP -->
		<div id="siteWrap">
            
            <!-- Our main UI -->
            <div id="gameWrapper" class="gameWrapper-Main"> <!-- background image classes Intro == gameWrapper-Main, War Room == gameWrapper-WarRoom -->
                <noscript>

                    <div style="width:960px; height:560px; background:url(Content/images/updateBrowser/bg.jpg) no-repeat; position:absolute; top:0; left:0; text-align:center;">
                        <div style="position:absolute; top:200px; left:170px; right:170px; bottom:0; text-align:center; line-height:24px;">
                        	<p style="font-size:14px; margin-bottom:48px; color:#fff;">Commander, it appears that you have javascript turned off!<br/>
                            <span style="font-size:12px;color:#868b60; margin-bottom:24px;">Our targeting systems rely on it being turned on to function properly. Perhaps you should enable it?</span></p>
                        </div>
                    </div>

                </noscript>

                <!-- Used when view content takes up the whole screen (i.e: In game or inital screens -->
                <div id="fullScreenTemplate" style="width:inherit; height:inherit;"></div>

                <!-- Used for war room screens -->
                <div id="twoColumnTemplate" style="position: absolute; top:0px; background:#333">
                    <div id="topLeft" style="position: relative; top: 0px;">

                    </div>

                    <div id="bottomLeft" style="position: relative;">

                    </div>

                    <div id="rightScreen" style="position: relative; left: 250px;">

                    </div>

                </div>
                
                <!-- inital loading modal -->
                 <div id="loadingModal" style="position: absolute; top:0px; left:0px; width: 100%; height:100%; visibility: hidden;">
                 	<div id="loading-bg" style="opacity: 1">
                    	<div id="loading-bar" style="background-position:0 0;"></div>
                    </div>
                </div>
            	<div id="systemDownModal" style="display:table; width:960px; height:560px; background:url(Content/images/updateBrowser/bg.jpg) no-repeat; position:absolute; top:0; left:0; text-align:center; vertical-align:middle; visibility: hidden;">
                	<h3 style="margin-top:250px;">We've lost communication with Command!</h3>
                    <p>We're working on getting server communication back up. Sending you in without it would be suicide.<br/>Come back later to see if we've resolved the issue.</p>

				</div>

                <div id="storeButtonWrap-persistant" style="visibility:visible;">
                	<div id="button-audio" class="button button-disabled" style="width:55px;"><div><div id="button-audio-label-wrap" class="audio-on"></div><p id="button-audio-label">Off</p></div></div>
                    <div id="button-about-persistant" class="button button-moss" style="width:65px;"><div id="button-login-text">about</div></div>
                </div>
                
                <!-- FACEBOOK LIKE -->
                <div style="position:absolute; bottom:-55px; left:95px; z-index:10000;">
					<iframe src="http://www.facebook.com/plugins/like.php?app_id=232725070094818&amp;href=http%3A%2F%2Fwww.tankster.com&amp;send=false&amp;layout=button_count&amp;width=90&amp;show_faces=false&amp;action=like&amp;colorscheme=dark&amp;font=lucida+grande&amp;height=21" scrolling="no" frameborder="0" style="border:none; overflow:hidden; width:90px; height:21px;" allowTransparency="true"></iframe>                </div>
                <!-- Google Plus -->
                <div style="position:absolute; bottom:-53px; left:190px; z-index:10000;">
	                <g:plusone size="small" count="true" href="http://www.tankster.net"></g:plusone>
				</div>
                <!-- Twitter -->
                <div style="position:absolute; bottom:-55px; left:-20px; z-index:10000;" >

					<a href="http://twitter.com/share" class="twitter-share-button" data-url="http://www.tankster.net" data-text="Check out Tankster - A new HTML5 game based on Scortched Earth #Tankster #HTML5 #Canvas #WindowsAzure" data-count="horizontal" data-via="gskinner" data-related="WindowsAzure:The official account for Windows Azure">Tweet</a><script type="text/javascript" src="http://platform.twitter.com/widgets.js"></script>
                
                </div>
            
                
            </div><!-- END Our Main UI -->

            <footer>
                <p>
                &copy; 2011 Microsoft | Privacy Statement | Terms of use | <a target="_blank" href="http://www.microsoft.com/library/toolbar/3.0/trademarks/en-us.mspx">Trademarks</a>
                <br />
                concept, development, and creative direction by <a target="_blank" href="http://www.gskinner.com">gskinner.com</a>
                <br />
                created using <a target="_blank" href="http://www.easeljs.com">EaselJS</a>
                <br />
                Illustration by <a target="_blank" href="http://pulpstudios.ca/">PULP studios</a>.
                <br />
                Powered By <a target="_blank" href="http://windowsazure.com/">Windows Azure</a>
                </p>
            </footer>

        </div><!-- END SITE WRAP -->

        <!--
        ####################################
        Views
        ####################################
        -->

        <!--
            Wrap all views in a hidden div.
        -->
        <div class="ui-helper-hidden" style="display: hidden; height: 0px;">
            
            <!--
            ####################################
            ####################################
            ####################################
            Invite Dialog View
            ####################################
            ####################################
            ####################################
            -->
            <div id="inviteDialogView" class="popup-wrap" style="width:500px; height:315px;">
                <div class="popup-header-frame-wrap">
                    <div class="frame-wood-top"></div>
                    <div class="frame-wood-right"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left"></div>
                    <div class="popup-header-wrap">
                        <h1>create invitational match</h1>
                    </div>
                </div>
                <div class="popup-content-frame" style="height:240px;">
                    <div class="frame-wood-right frame-notop"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left frame-notop"></div>
                    <div class="popup-content-wrap" style="text-align:center">
                        <p>Copy the following link:</p>
                        <p id="linkContainer"></p>
                        <p>Send it to your friends:</p>
                        <p>Post it on Facebook, Tweet it on Twitter, E-mail it</p>
                        <p>*Your invitation will remain open as long as you are still in the battle queue.</p>
                        <div class="center-margins" style="width:350px;">
                            <div id="confirmBtn" class="button button-green left" style="width:170px;"><div>start battle queue</div></div>
                            <div id="cancelBtn" class="button button-red right" style="width:170px;"><div>cancel invitation</div></div>
                        </div>
                    </div>
                </div>
            </div>

            <!--
            ####################################
            ####################################
            ####################################
            Achievement Dialog View
            ####################################
            ####################################
            ####################################
            -->
            <div id="achievementDialog" class="popup-wrap" style="width:470px; height:500px;">
                <div class="popup-header-frame-wrap">
                    <div class="frame-wood-top"></div>
                    <div class="frame-wood-right"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left"></div>
                    <div class="popup-header-wrap">
                        <h1>achievements</h1>
                        <div id="closeBtn" class="button button-red right"><div>close</div></div>
                    </div>
                </div>
                <div class="popup-content-frame" style="height:435px; padding:0px 10px 0px 10px;">
                    <div class="frame-wood-right frame-notop"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left frame-notop"></div>
                    <div class="popup-content-wrap" style="padding:10px 0;">

                        <!-- populated by AchievementDialog.js -->
                        <div id="achievementsContainer" style="width:100%; height:380px; overflow:auto;"></div>

                        <div class="paginationWrap">
                            <div id="prevBtn" class="button button-moss"><div>&laquo;</div></div>
                            <p id="pageLbl">PAGE 1/9</p>
                            <div id="nextBtn" class="button button-moss"><div>&raquo;</div></div>
                        </div>
                    </div>
                </div>
            </div>

            <!--
            ####################################
            ####################################
            ####################################
            Pre-game Weapons Purchase Dialog View
            ####################################
            ####################################
            ####################################
            -->
            <div id="mainWeaponSelectView" class="popup-wrap center-margins" style="width:940px; height:520px;">
                <div class="popup-header-frame-wrap">
                    <div class="frame-wood-top"></div>
                    <div class="frame-wood-right"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left"></div>
                    <div class="popup-header-wrap">
                        <h1 id="mainWeaponsTitle">{MAIN_WEAPON_DIALOG_TITLE}</h1>
                        <p class="popup-header-text" style="left:300px;width:600px;">Click on a Weapon to add it to your arsenal <strong>(Shift Click To Remove)</strong><br/> get the weapons and Power-Ups you want before you run out of Cash.</p>
                        <div id="leaveBattleBtn" class="button button-red right"><div>leave battle</div></div>
                    </div>
                </div>
                <div class="popup-content-frame" style="height:455px; padding:0;">
                    <div class="frame-wood-right frame-notop"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left frame-notop"></div>
                    <div class="popup-content-wrap">

                        <div id="resetContainer" class="weapon-frame-wrapper" style="width:142px; height:180px; left:10px;top:0px; padding:0;">
                            <div class="frame-wood-bottom"></div>
                            <p style="position:absolute; top:110px; width:100%; text-align:center;">{DEFAULT_WEAPON_LBL}</p>
                            <div id="resetBtn" class="button button-red" style="width:116px; position:absolute; bottom:20px; left:10px; right:auto;"><div>RESET</div></div>
                        </div>

                        <div class="weapon-frame-wrapper" style="width:142px; height:255px; left:10px; top:180px; padding-top:10px;">
                            <p>{REMAINING_BUDGET_LBL}</p>
                            <p id="budget">$10,000</p>
                            <p>{CASH_PER_TURN_LBL}</p>
                            <p id="cashPerTurn">$50</p>
                            <div style="width: 122px; text-align:center; position:absolute; bottom:10px; left:10px;">
                            
                                <div id="countdownContainer" class="text-effect-shadow" style="color:#cccc99;"></div> <!-- Populated by Countdown timer -->
                                <p class="timer-title text-effect-shadow" style="color:#ffffff; font-size:12px;">{UNTIL_BATTLE_LBL}</p>
                                <!-- Ready Button - Only visible in Practice Weapon Selection Screen, when pressed it will jump to the next players weapon selection screen -->
                                <div id="readyBtn" class="button button-green" style="width:116px; position:absolute; bottom:65px; left:0px; right:auto;"><div>Done</div></div>
                            </div>
                        </div>
                        
                        <div id="weaponsContainer" class="weapon-frame-wrapper" style="width:670px; height:445px; left:152px;">
                            <div class="frame-wood-left frame-notop frame-nobottom"></div>
                        </div>


                        <div id="powersContainer" class="weapon-frame-wrapper" style="width:100px; height:445px; right:10px; padding-left:10px;">
                            <div class="frame-wood-left frame-notop frame-nobottom"></div>
                        </div>
                    </div>
                </div>
            </div>

            <!--
            ####################################
            ####################################
            ####################################
            Weapon Select Dialog View
            ####################################
            ####################################
            ####################################
            -->
            <div id="weaponSelectView" class="popup-wrap center-margins" style="width:940px; height:520px;">
                <div class="popup-header-frame-wrap">
                    <div class="frame-wood-top"></div>
                    <div class="frame-wood-right"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left"></div>
                    <div class="popup-header-wrap">
                        <h1>weapon and powerup selection</h1>
                        <p class="popup-header-text" style="left:300px;">Click on a Weapon to select it.<br/>You can also purchase new weapons and powerups.</p>
                        <div id="closeBtn" class="button button-red right"><div>close</div></div>
                        <div id="leaveBattleBtn" class="button button-moss right" style="margin-right:5px;"><div>leave battle</div></div>
                    </div>
                </div>
                <div class="popup-content-frame" style="height:455px; padding:0;">
                    <div class="frame-wood-right frame-notop"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left frame-notop"></div>
                    <div class="popup-content-wrap">
                        
                        <div id="resetContainer" class="weapon-frame-wrapper" style="width:142px; height:170px; left:10px;top:0px; padding:0;">
                            <div class="frame-wood-bottom"></div>
                            <p style="position:absolute; top:110px; width:100%; text-align:center;">{DEFAULT_WEAPON_LBL}</p>
                        </div>

                        <div class="weapon-frame-wrapper" style="width:142px; height:265px; left:10px; top:170px; padding-top:10px;">
                            <p>{REMAINING_BUDGET_LBL}</p>
                            <p id="budget">$10,000</p>
                            <p>{CASH_PER_TURN_LBL}</p>
                            <p id="cashPerTurn">$50</p>
                            <div style="position:absolute; bottom:10px; left:10px;">
                                <div id="countdownContainer" class="text-effect-shadow" style="color:#cccc99;"></div> <!-- Populated by Countdown timer -->
                                <!--<p class="timer-title text-effect-shadow" style="color:#ffffff; font-size:12px;">{UNTIL_BATTLE_LBL}</p>	-->
                            </div>
                        </div>
                        
                        <div id="weaponsContainer" class="weapon-frame-wrapper" style="width:670px; height:445px; left:152px;">
                            <div class="frame-wood-left frame-notop frame-nobottom"></div>
                        </div>
                        
                        <div id="powersContainer" class="weapon-frame-wrapper" style="width:100px; height:445px; right:10px; padding-left:10px;">
                            <div class="frame-wood-left frame-notop frame-nobottom"></div>
                        </div>
                    </div>
                </div>
            </div>

            <!--
            ####################################
            ####################################
            ####################################
            Victory/Defeat/TimedOut Dialog View
            ####################################
            ####################################
            ####################################
            -->
            <div id="victoryView" class="popup-wrap endGame" style="width:640px;height:300px;">
                <div class="frame-wrapper frame-wrapper-victory bg-default-dark" style="height:162px;"> <!-- to change bg change class between frame-wrapper-victory, frame-wrapper-defeat, frame-wrapper-timedout -->
                    <div class="frame-wood-top"></div>
                    <div class="frame-wood-right"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left"></div>
                    <div class="frame-content">
                        <p id="headerLabel" class="text-effect-shadow"></p>
                        <p id="playerPerformance" class="text-effect-shadow"></p>
                        <!-- SHARE BAR -->
                        <div id="shareBarEndGame" style="margin:24px auto auto auto; text-align:center;">
                            <p id="shareLabel">Share Your Victory:</p>
                            <a href="http://www.facebook.com/sharer.php?u=tankster.net&t=Tankster" target="_blank">
                            	<img source="Content/images/shareIcons/facebook.png" title="Share on Facebook"></a>
                            <a href="http://twitter.com/share?text=Check out Tankster, the fun new turn based shooting game built with HTML5! %23Tankster&url=http://tankster.net" target="_blank">
                            	<img source="Content/images/shareIcons/twitter.png" title="Share on Twitter"></a>
                            <a href="http://profile.live.com/badge/?url=http://tankster.net&title=Tankster&description=Check out Tankster, the fun new turn based shooting game built with HTML5!" target="_blank">
                            	<img source="Content/images/shareIcons/messenger.png" title="Share with Messenger"></a>
                            <a href="http://www.stumbleupon.com/submit?url=http://tankster.net&title=Check out Tankster, the fun new turn based shooting game built with HTML5!" target="_blank">
                            	<img source="Content/images/shareIcons/stumble.png" title="Share on StumbleUpon" ></a>
                            <a href="http://www.digg.com/submit?url=http://tankster.net&title=Check out Tankster, the fun new turn based shooting game built with HTML5!" target="_blank">
                            	<img source="Content/images/shareIcons//digg.png" title="Share on Digg"></a>
                            <a href="http://www.delicious.com/submit?url=http://tankster.net&title=Check out Tankster, the fun new turn based shooting game built with HTML5!" target="_blank"><img source="Content/images/shareIcons/delicious.png" title="Share on Delicious"></a>
                            <a href="http://www.reddit.com/submit?url=http://tankster.net&title=Check out Tankster, the fun new turn based shooting game built with HTML5!" target="_blank">
                            	<img source="Content/images/shareIcons/reddit.png" title="Share on Reddit"></a>
                        </div>
                    </div>
                    
                </div>
                <div class="frame-wrapper" style="height:115px;">
                    <div class="frame-wood-right frame-notop"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left frame-notop"></div>
                    <div style="frame-content">
                        <div class="kvatr-wrap">
                            <div class="rank"><h3>Rank</h3><br><p>248</p></div>
                            <div class="victories"><h3>Victories</h3><br/><p>9999</p></div>
                            <div class="kills"><h3>Kills</h3><br/><p>9999</p></div>
                            <div class="accuracy"><h3>Accuracy</h3><br/><p>84.1%</p></div>
                            <div class="terrain"><h3>Terrain</h3><br/><p>9999k</p></div>
                        </div>
                        <div id="continueBtn" class="button button-green buttonWidth-140" style="position:absolute; bottom:20px; left:250px;"><div>CONTINUE</div></div>
                    </div>
                </div>
            </div>

            <!--
            ####################################
            ####################################
            ####################################
            Edit Character Dialog View
            ####################################
            ####################################
            ####################################
            -->
            <div id="editCharacterView" class="popup-wrap" style="width:302px; height:455px;">
                <div class="popup-header-frame-wrap">
                    <div class="frame-wood-top"></div>
                    <div class="frame-wood-right"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left"></div>
                    <div class="popup-header-wrap">
                        <h1>edit character</h1>
                    </div>
                </div>
                <div class="popup-content-frame" style="height:380px;">
                    <div class="frame-wood-right frame-notop"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left frame-notop"></div>
                    <div class="popup-content-wrap" style="height:370px; width:262px; position:relative;">

                        <p id="characterTitleText" style="text-align:left;">NAME</p>
                        <input id="nameInput" type="text" style="width:241px;" /><br/>

                        <div id="characterPreviewWrap">
                            <div id="characterContainer"></div>
							<div style="width:121px; position:absolute; top:140px; left:6px;">
                            	<p>CHARACTER</p>
                            	<div id="prevCharBtn" class="button button-moss left" style="width:55px;"><div><span style="font-size:18px; line-height:10px;">&laquo;</span></div></div>
                            	<div id="nextCharBtn" class="button button-moss right" style="width:55px;"><div><span style="font-size:18px; line-height:10px;">&raquo;</span></div></div>
                            </div>

                            <div id="tankContainer">
								<canvas id="characterTankCanvas"></canvas>
                            </div>
							<div style="width:121px; position:absolute; top:140px; right:10px;">
                                <!--<p>TANK</p>
                                <div id="prevTankBtn" class="button button-disabled left" style="width:55px;"><div><span style="font-size:18px; line-height:10px;">&laquo;</span></div></div>
                                <div id="nextTankBtn" class="button button-disabled right" style="width:55px;"><div><span style="font-size:18px; line-height:10px;">&raquo;</span></div></div>
                                <p class="clear" style="padding-top:10px;">DECAL</p>-->
                                <p>DECAL</p>
                                <div id="prevDecalBtn" class="button button-moss left" style="width:55px;"><div><span style="font-size:18px; line-height:10px;">&laquo;</span></div></div>
                                <div id="nextDecalBtn" class="button button-moss right" style="width:55px;"><div><span style="font-size:18px; line-height:10px;">&raquo;</span></div></div>
							</div>
                        </div>
                        
                        <div id="confirmBtn" class="button button-green" style="position:absolute; width:120px; bottom:0px; left:6px;"><div>confirm</div></div>
                        <div id="cancelBtn" class="button button-red" style="position:absolute; width:120px; bottom:0px;  right:6px;"><div>cancel</div></div>

                    </div>
                </div>
            </div>

            <!--
            ####################################
            ####################################
            ####################################
            Inventory Dialog View
            ####################################
            ####################################
            ####################################
            -->
            <div id="inventoryView" class="popup-wrap" style="width:470px; height:490px;">
                <div class="popup-header-frame-wrap">
                    <div class="frame-wood-top"></div>
                    <div class="frame-wood-right"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left"></div>
                    <div class="popup-header-wrap">
                        <h1>inventory</h1>
                            <div id="closeBtn" class="button button-red right"><div>close</div></div>
                            <!--<div id="storeBtn" class="button button-green right" style="margin-right:5px;"><div>store</div></div>-->
                            <label id="resourcesLabel">x0</label>
                            <img id="creditsBricksImg" source="Content/images/common/icons/credits-bricks-44x25.png" width="44" height="25" alt="credit icon"/>
                    </div>
                </div>
                <div class="popup-content-frame" style="height:425px; padding:0px 10px 0 10px;">
                    <div class="frame-wood-right frame-notop"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left frame-notop"></div>
                    <div class="popup-content-wrap" style="padding:10px 0;">
                        <!-- populated by InventoryDialog.js -->
                        <div id="inventoryContainer" style="width:100%; height:380px; overflow:auto;"></div>

                        <div class="paginationWrap">
                            <div id="prevBtn" class="button button-moss"><div><span style="font-size:18px; line-height:10px;">&laquo;</span></div></div>
                            <p id="pageLbl">PAGE 1/9</p>
                            <div id="nextBtn" class="button button-moss"><div><span style="font-size:18px; line-height:10px;">&raquo;</span></div></div>
                        </div>
                    </div>
                </div>
            </div>

            <!--
            ####################################
            ####################################
            ####################################
            View to show behind the inital game weapon select screen.
            ####################################
            ####################################
            ####################################
            -->
            <div id="gameWeaponSelectBackground" style="width: 100%; height: 100%">

            </div>
            
            <!--
            ####################################
            ####################################
            ####################################
            Leaderboard Dialog View
            ####################################
            ####################################
            ####################################
            -->
            <div id="leaderboardDialogView" class="popup-wrap center-magins" style="width:671px; height:535px;">
                <div class="popup-header-frame-wrap">
                    <div class="frame-wood-top"></div>
                    <div class="frame-wood-right"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left"></div>
                    <div class="popup-header-wrap">
                        <h1>Leaderboard</h1>
                        <div id="closeBtn" class="button button-red right"><div>close</div></div>
                    </div>
                </div>
                <div class="popup-content-frame" style="height:470px; padding:0">
                    <div class="frame-wood-right frame-notop"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left frame-notop"></div>
                    <div class="popup-content-wrap" style="padding:0px 10px 0 10px;">
                    	<ul id="leaderboardHeader">
                        	<li class="empty" style="width:189px; border-left:none;"></li>
                        	<li id="rankBtn" class="active">
                                <img source="Content/images/common/icons/kvatr-rank-large.png"/>
                                <p>{RANK_TAB}</p>                            
                            </li>
                        	<li id="victoryBtn">
                                <img source="Content/images/common/icons/kvatr-victory-large.png"/>
                                <p>{VICTORIES_TAB}</p>                            
                            </li>
                        	<li id="killsBtn">
                                <img source="Content/images/common/icons/kvatr-kills-large.png"/>
                                <p>{KILLS_TAB}</p>                            
                            </li>
                        	<li id="accuracyBtn">
                                <img source="Content/images/common/icons/kvatr-accuracy-large.png"/>
                                <p>{ACCURACY_TAB}</p>                            
                            </li>
                        	<li id="terrainBtn">
                                <img source="Content/images/common/icons/kvatr-terrain-large.png"/>
                                <p>{TERRAFORM_TAB}</p>
                            </li>
                        </ul>
                        
                        <!-- List populated in Leaderboard.js -->
                        <ul id="scorelist">
                        	<li id="scorelistContent">
                                
                            </li>
                        </ul>
                      </div>
                </div>
            </div>

            <!--
            ####################################
            ####################################
            ####################################
            Main view
            ####################################
            ####################################
            ####################################
            -->
            <div id="mainView" style="width: 100%; height: 560px;">
            
            	<!-- PLAY Multi/single game -->
                
                <div class="woodBox-wrap" style="width:490px; height:70px; bottom:55px; left:235px; z-index:200;">
                    <div class="frame-wood-top"></div>
                    <div class="frame-wood-right"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left"></div>
                    <div class="woodBox-content-wrap">
                        <div id="playMultiplayerBtn" class="button button-moss buttonWidth-220 left"><div>{PLAY_MULTIPLYAER_GAME_BTN}</div></div>
                        <div id="playSinglePlayerBtn" class="button button-moss buttonWidth-220 right"><div>{PLAY_SINGLE_PLAYER_BTN}</div></div>
                    </div>
                </div>
				<!-- END PLAY Multi/single game -->
        		<!-- Slide Out Login Options -->
                <div  id="loginTypesWrapper" class="popup-wrap" style="position:absolute; bottom:120px; left:235px; width:490px; height:115px; z-index:100; box-shadow:none; -moz-box-shadow:none; -webkit-box-shadow:none;">
                    <div class="popup-header-frame-wrap">
                        <div class="frame-wood-top"></div>
                        <div class="frame-wood-right"></div>
                        <div class="frame-wood-bottom"></div>
                        <div class="frame-wood-left"></div>
                        <div class="popup-header-wrap">
                            <h1>{LOGIN_LBL}</h1>
                        </div>
                    </div>
                    <div class="popup-content-frame" style="height:65px;">
                        <div class="frame-wood-right frame-notop frame-nobottom"></div>
                        <div class="frame-wood-left frame-notop frame-nobottom"></div>
                        <div class="popup-content-wrap">
                            <div class="button button-moss buttonWidth-220" style="position:absolute; top:10px; left:20px;"><div id="loginType0">loginOne</div></div>
                            <div class="button button-moss buttonWidth-220" style="position:absolute; top:10px; right:20px;"><div id="loginType1">loginTwo</div></div>
                            <div class="button button-moss buttonWidth-220" style="visibility: collapse; position:absolute; bottom:15px; left:135px;"><div id="guestLoginBtn">{LOGIN_GUEST_BTN}</div></div>
                        </div>
                    </div>
                </div>
        		<!-- END Slide Out Login Options -->

                <div id="aboutGameBtn" class="button button-brown buttonWidth-220" style="position:absolute; bottom:20px; left:370px;"><div>{ABOUT_GAME_BTN}</div></div>
            </div>

            <!--
            ####################################
            ####################################
            ####################################
            Game navigation
            ####################################
            ####################################
            ####################################
            -->
            <!-- Game navigation -->
            <div id="gameNavigation">
            
            	<!-- Character Area -->
                <div id="gameNavCharacterWrap">
                	<div id="avatar"></div>
                    <div id="nameEditableLabel">character name</div>
                    <img id="creditsBricksImg" source="Content/images/common/icons/credits-bricks-44x25.png" width="44" height="25" alt="credit icon" style="position:absolute; top:70px; left:115px;">
                    <label id="resourcesLabel">x0</label>
                  <div class="button button-disabled"><div>Buy Credits</div></div>
                </div>

				<!-- Primary Nav and Stats -->
                <div id="gameNavWrap">
                	<div class="kvatr-wrap">
                    	<div id="rank" class="rank">-</div>
                    	<div id="victories" class="victories">-</div>
                    	<div id="kills" class="kills">-</div>
                    	<div id="accuracy" class="accuracy">-</div>
                    	<div id="terrain" class="terrain">-</div>
                    </div>
                    <div id="viewLeaderBoardBtn" class="button button-moss buttonWidth-140" style="position:absolute; bottom:46px; left:10px;"><div>{RANKING_BTN}</div></div>
                    <div id="viewInventoryBtn" class="button button-moss buttonWidth-140" style="position:absolute; bottom:46px; right:10px;"><div>{INVENTORY_BTN}</div></div>
                    <!--<div id="viewAcheivementsBtn" class="button button-disabled buttonWidth-140" style="position:absolute; bottom:46px; right:10px;"><div>{ACHEIEVEMENTS_BTN}</div></div>-->
                    <div id="editCharacterBtn" class="button button-moss" style="position:absolute; bottom:10px; left:10px; width:286px;"><div>{MY_CHARACTER_BTN}</div></div>
                </div>
            </div>
            <!-- END Game navigation -->

            <div id="noCanvasView" style="width:960px; height:560px; background:url(Content/images/updateBrowser/bg.jpg) no-repeat; position:absolute; top:0; left:0; text-align:center; vertical-align:middle;">
            	<div style="height:141px; position:absolute; top:140px; left:170px; right:170px; bottom:auto; vertical-align:bottom;">
                	<a href="http://windows.microsoft.com/en-CA/internet-explorer/downloads/ie" target="_blank"><img src="Content/images/updateBrowser/internetexplorer.png" width="125" height="117" alt="Download Microsoft Internet Explorer"></a>
                	<a href="http://www.google.com/chrome/" target="_blank"><img src="Content/images/updateBrowser/chrome.png" width="117" height="116" alt="Download Google Chrome"></a>
           	    	<a href="http://www.mozilla.com/en-US/firefox/new/" target="_blank"><img src="Content/images/updateBrowser/firefox.png" width="127" height="127" alt="Download Mozilla Firefox"></a>
                	<a href="http://www.apple.com/safari/download/" target="_blank"><img src="Content/images/updateBrowser/safari.png" width="120" height="127" alt="Download Apple Safari"></a>
                </div>
<div style="position:absolute; top:300px; left:170px; right:170px; bottom:0; text-align:center; line-height:24px;">
                	<p style="font-size:14px; margin-bottom:48px; color:#fff;">Sorry, Your browser is an antique<br/>
                    <span style="font-size:12px;color:#868b60; margin-bottom:24px;">If you're going to play with this kind of firepower, you're gonna need a modern browser!</span></p>
                	<p style="font-size:12px; margin:0; color:#fff;">Upgrade today!<br/>
                    <span style="font-size:12px; color:#868b60;"><a href="http://windows.microsoft.com/en-CA/internet-explorer/downloads/ie" title="Download Microsoft Internet Explorer" target="_blank">Internet Explorer</a>, <a href="http://www.google.com/chrome/" title="Download Google Chrome" target="_blank">Chrome</a>, <a href="http://www.mozilla.com/en-US/firefox/new/" title="Download Mozilla Firefox" target="_blank">Firefox</a>, <a href="http://www.apple.com/safari/download/" title="Download Apple Safari" target="_blank">Safari</a></span></p>

                </div>
          </div>
			
            <!-- Invitation to game -->
            <div id="mainInviteView" style="position: absolute; width: 100%; height:100%;">

                <label style="font-size: 25">
                    {INVITED_DESC}
                </label>



        		<!-- Slide Out Login Options -->
                <div  id="loginTypesWrapper" class="popup-wrap" style="position:absolute; bottom:55px; left:235px; width:490px; height:160px; z-index:100; box-shadow:none; -moz-box-shadow:none; -webkit-box-shadow:none;">
                    <div class="popup-header-frame-wrap">
                        <div class="frame-wood-top"></div>
                        <div class="frame-wood-right"></div>
                        <div class="frame-wood-bottom"></div>
                        <div class="frame-wood-left"></div>
                        <div class="popup-header-wrap">
                            <h1>{LOGIN_LBL}</h1>
                        </div>
                    </div>
                    <div class="popup-content-frame" style="height:75px;">
                        <div class="frame-wood-right frame-notop"></div>
                        <div class="frame-wood-left frame-notop"></div>
                        <div class="frame-wood-bottom"></div>
                        <div class="popup-content-wrap">
                            <div  id="fbLogin" class="button button-moss buttonWidth-220" style="position:absolute; top:10px; left:20px;"><div id="loginType0">{LOGIN_FACEBOOK_BTN}</div></div>
                            <div id="liveLogin" class="button button-moss buttonWidth-220" style="position:absolute; top:10px; right:20px;"><div id="loginType1">{LOGIN_LIVE_BTN}</div></div>
                            <div id="guestLogin" class="button button-moss buttonWidth-220" style="position:absolute; bottom:20px; left:135px;"><div id="guestLoginBtn">{LOGIN_GUEST_BTN}</div></div>
                        </div>
                    </div>
                </div>
        		<!-- END Slide Out Login Options -->

                <div id="aboutGameBtn" class="button button-brown buttonWidth-220" style="position:absolute; bottom:20px; left:370px;"><div>{ABOUT_GAME_BTN}</div></div>


            </div>
            <!-- END Invitation to game -->

            <!--
            ####################################
            ####################################
            ####################################
            About Dialog View
            ####################################
            ####################################
            ####################################
            -->
            <div id="aboutView" class="popup-wrap center-magins" style="width:450px; height:540px;">
                <div class="popup-header-frame-wrap">
                    <div class="frame-wood-top"></div>
                    <div class="frame-wood-right"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left"></div>
                    <div class="popup-header-wrap">
                        <h1>{ABOUT_TITLE}</h1>
                        <div id="closeBtn" class="button button-red right"><div>close</div></div>
                    </div>
                </div>
                <div class="popup-content-frame" style="height:455px;">
                    <div class="frame-wood-right frame-notop"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left frame-notop"></div>
                    <div class="popup-content-wrap">
                        <h3>THE STORY</h3>
                        <p>War has erupted on the beach! Dominate other players using a range of weapons and defenses that demolish the players, as well as the surrounding environment. If you can't claim a spot in the leader board as the best tank in the game, humiliate your opponent by giving them an embarrassing decal or tank design, or hone your craft in practice battles.</p>
                        
                        <h3>ABOUT GAME</h3>
                        <p>Built in JavaScript and HTML5 utilizing Canvas and Audio, and built upon the flexible Azure server framework, Tankster aims to be a fun, engaging, multiplayer experience that pushes the limits of browser-based gaming.</p>

                        <h3>HOW TO PLAY</h3>
                        <ul>
                            <li>Buy weapons using an allotted budget at the beginning of the game.</li>
                            <li>Press and drag a tank to initiate a shot. Drag in the direction you wish to shoot, pulling further away for a longer shot.</li>
                            <li>Use power-ups such as shields and parachutes when it is your turn to minimize damage when hit by other players.</li>
                            <li>Last one standing wins!</li>
                        </ul>
                        
                        <h3>GAME CREDITS</h3>
                        <p>Designed, developed and built by <a href="http://gskinner.com">gskinner.com</a>, Illustration by <a href="http://pulpstudios.ca">Pulp Studios</a>, Sound by <a href="mailto:washingtron@gmail.com">Washingtron</a></p>

                    </div>
                </div>
            </div>

             <!--
             ####################################
             ####################################
             ####################################
             In game turn dialog
             ####################################
             ####################################
             ####################################
             -->
            <div id="turnDialogue" class="bg-default-dark" style="width:560px; height:120px;">
                <h3 id="headerText" class="text-effect-shadow"></h3>
                <p id="bodyText" class="text-effect-shadow"></p>
                <div id="turnavatar"></div>
            </div>

            <!--
             ####################################
             ####################################
             ####################################
             Pranks Dialog
             ####################################
             ####################################
             ####################################
             -->          
            <div id="pranksDialog" class="popup-wrap" style="width:280px; height:325px;">
                <div class="popup-header-frame-wrap">
                    <div class="frame-wood-top"></div>
                    <div class="frame-wood-right"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left"></div>
                    <div class="popup-header-wrap">
                        <h1>Pranks</h1>
                        <div id="closeBtn" class="button button-red right" style="width:120px;"><div>close</div></div>
                    </div>
                </div>
                <div class="popup-content-frame" style="height:260px; padding:0px 10px 0px 10px;">
                    <div class="frame-wood-right frame-notop"></div>
                    <div class="frame-wood-bottom"></div>
                    <div class="frame-wood-left frame-notop"></div>
                    <div class="popup-content-wrap" style="padding:10px 0; text-align:center;">
                        <p id="headerText" class="text-effect-shadow"></p>

                        <p id="recentContent">THIS TANK HAS BEEN RECENTLY PRANKED. YOU CAN PRANK THIS TANK IN</p>

                        <div id="selectContent">
                            <p>CLICK ON A DECORATION TO USE IT.</p>
                            <ul id="prankList" style="margin-top:10px;"></ul>
                        </div>

                        <div id="noneContent" style="text-align:center;">
                            <p>PRANKS CAN BE PURCHASED FROM THE STORE AT ANY TIME - ALL YOU NEED ARE SOME CREDITS.</p>
                            <p>ONCE YOU BUY A PRANK YOU CAN USE IT FOREVER. COLLECT THEM ALL!</p>
                            <div id="storeBtn" class="button button-green buttonWidth-140"><div>{VISIT_STORE_BTN}</div></div>
                        </div>
                    </div>
                </div>
            </div>
            
             <!--
             ####################################
             ####################################
             ####################################
             Select Game Type Bottom Nav
             ####################################
             ####################################
             ####################################
             -->

			<!-- BATTLE QUEUE -->
             <div id="selectGameNavigation" class="battleQueue-wrap">
             	<ul class="tabs" style="position:absolute; top:-10px; left:-30px; width:310px;">
                	<li id="onlineMultiplayer" class="first" style="width:84px;">Online Multiplayer</li>
                    <li id="localGame" class="last active" style="width:83px;">Local Game</li>
                </ul>

                <!-- Different select game views -->
                <!--<div id="skirmishContent" style="top:25px;">
                    <p>{SKIRMISH_CONTENT}</p>
                </div>
                
                <div id="inviteContent" style="top:25px;">
                    <p>{INVITE_CONTENT}</p>
                </div>-->
                
                <div id="onlineContent" style="position:relative; top:45px; left:0; right:0; display:table-cell; vertical-align:middle; height:200px;"> 
                    <!-- Toggled by SelectGameNavigation.js  -->
                    <!--<p>YOU MUST LOGIN USING ONE OF THE FOLLOWING SERVICES TO PLAY AN ONLINE MULTIPLAYER GAME.</p>-->
                    <!--<p>BATTLE HEAD-TO-HEAD AGAINST PLAYERS IN ONLINE SAND BLASTING MADNESS.</p>-->
                    <!--<p id="findingOpponentsLabel">FINDING_OPPONENTS <span class="text-color-white text-case-none">x0</span></p>
                    <p class="timer-title text-color-white text-effect-shadow" style="margin-top:80px;">Waiting for more players...</p>-->
                </div>

                <div id="practiceContent" style="top:45px;">
                    <input id="playerInput0" type="text" disabled="true" style="top:0px;"/>
                    <div id="toggleHuman0" class="toggle first human disabled" style="top:5px;"><div><div></div></div></div><div id="toggleComputer0" class="toggle last ai" style="top:5px;"><div><div></div></div></div><br/>
                    <input id="playerInput1" disabled="true" type="text" style="top:40px;"/>
                    <div id="toggleHuman1" class="toggle first human" style="top:45px;"><div><div></div></div></div><div id="toggleComputer1" class="toggle last ai disabled" style="top:45px;"><div><div></div></div></div><br/>
                    <input id="playerInput2" disabled="true" type="text" style="top:80px;"/>
                    <div id="toggleHuman2" class="toggle first human" style="top:85px;"><div><div></div></div></div><div id="toggleComputer2" class="toggle last ai disabled" style="top:85px;"><div><div></div></div></div><br/>
                    <input id="playerInput3" disabled="true" type="text" style="top:120px;"/>
                    <div id="toggleHuman3" class="toggle first human" style="top:125px;"><div><div></div></div></div><div id="toggleComputer3" class="toggle last ai disabled" style="top:125px;"><div><div></div></div></div><br/>
                    <input id="playerInput4" disabled="true" type="text" style="top:160px;"/>
                    <div id="toggleHuman4" class="toggle first human" style="top:165px;"><div><div></div></div></div><div id="toggleComputer4" class="toggle last ai disabled" style="top:165px;"><div><div></div></div></div><br/>
                </div>

                <div class="button button-green buttonWidth-290" style="position:absolute; left:0; bottom:0;"><div id="startBtn">PLAY LOCAL GAME</div></div>

                <!-- BUTTON LABEL CHANGES DEPENDING ON SELECTION -->
                <div class="button button-red" style="position:absolute; left:0; bottom:0;"><div id="logoutBtn">LOGOUT</div></div>
                <div class="button button-green buttonWidth-220" style="position:absolute; right:0; bottom:0;"><div id="actionBtn">BUTTON</div></div>

				<!-- Login buttons, visibility is toggled from code. -->             
				<div class="button button-green buttonWidth-290" style="position:absolute; left:0; bottom:0;"><div id="facebookLoginbtn">Login with Facebook</div></div>
				<div class="button button-green buttonWidth-290" style="position:absolute; left:0; bottom:30px;"><div id="liveLoginbtn">Login with Windows Live ID</div></div>
				 
            </div>


			<!-- END BATTLE QUEUE - SELECT GAME -->

             <div id="skirmishGameTypeNavigation" class="battleQueue-wrap" style="text-align:center;">
                <h3>Battle Queue <span class="text-color-sand">{SKIRMISH_LBL}</span></h3>

                <p id="findingOpponentsLabel">FINDING_OPPONENTS <span class="text-color-white text-case-none">x0</span></p>
                 
                <p class="timer-title text-color-white text-effect-shadow" style="margin-top:80px;">Waiting for more players...</p><!-- {UNTIL_WEAPON_LBL} -->

                <div id="leaveQueueBtn" class="button button-red buttonWidth-290" style="position:absolute; left:0; bottom:0;"><div>{LEAVE_QUEUE_BTN}</div></div>
                
             </div>

            <!--
            ####################################
            ####################################
            ####################################
            Main game-play UI wrapper
            ####################################
            ####################################
            ####################################
            -->
            <div id="gameCanvasHolder" class="gameCanvasHolder" style="width:960px;height:500px;background-color:#9CC;">
                <!-- wdg:: temp ui for testing -->
                <div class="description">
                    <!-- CHARACTER PANEL IN GAME -------->
                    <div id="gameChromeDiv" style="z-index:500;">

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

                        <div id="sendMessageDiv">
                            <input id="chatMessageInput" maxlength="60" type="text">
                            <div id="sendMessageButton" class="button button-moss"><div>{SEND_MESSAGE_BTN}</div></div>
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
                    </div>
                    <!-- END CHARACTER PANEL IN GAME -------->
                    
                    
                    
                                        
                    <!-- Actual gameplay area -->
                    <!--<div id="canvasHolder" style="width:960px;height:500px; background-image:url(images/game/backgrounds/bg-03.jpg); background-repeat:no-repeat; background-position: top left; overflow: hidden;">
						<canvas id ="gameCanvas"></canvas>
					</div>-->

                    <div id="canvasHolder" style="width:960px; height:500px;
                        background:#2259a2 url(Content/images/game/gameBg-back.jpg);
                        background-position:0px 0px;
                        background-repeat:no-repeat; 
                        overflow: hidden;
                        position: relative;">

                        <div id="parallaxMidground" style="width:960px; height:500px; 
                        background:url(Content/images/game/gameBg-mid.png);
                        background-position:0px 7px;
                        background-repeat:no-repeat; 
                        overflow: hidden;
                        position: absolute;"></div>

                        <div id="parallaxForeground" style="width:960px; height:500px; 
                        background:url(Content/images/game/gameBg-fore.png);
                        background-position:0px 246px;
                        background-repeat:no-repeat; 
                        overflow: hidden;
                        position: absolute;"></div>

                        <canvas id ="gameCanvas" style="position:absolute; z-index:2;"></canvas>

                        <div id="uiTimer" style="position:absolute; top:10px; text-align:center; width:100%; font-size:30px; display:none;">30 sec</div>
                    </div>


                </div>
            </div>

        </div>
                    
    <div id="DEBUGME" style="color:#000; visibility:visible;">
	<!-- 
document.getElementById("DEBUGME").innerHTML += xxxxxxx + "<br/>";
	-->
	</div>

	</body>
    
    <script type="text/javascript">
      var _gaq = _gaq || [];
      _gaq.push(['_setAccount', 'UA-24611114-1']);
      _gaq.push(['_trackPageview', '/home']);
      
      (function() {
        var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
        ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
        var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
      })();
     
     (function() {
        var plusOne = document.createElement('script'); plusOne.type = 'text/javascript'; plusOne.async = true;
        plusOne.src = 'https://apis.google.com/js/plusone.js';
        var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(plusOne, s);
      })();

    </script>

</html>
