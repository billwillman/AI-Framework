;---------------------------------------------------------------------------
; Format:
; [Statedef STATENO]
; type = ?      S/C/A/L  stand/crouch/air/liedown
; movetype = ?  I/A/H    idle/attack/gethit
; physics = ?   S/C/A/N  stand/crouch/air/none
; juggle = ?             air juggle points move requires
;
; [State STATENO, ?]     ? - any number you choose
; type = ?
; ...


[Statedef 170]
type = S
anim = 170

[State 170, snd]
type = PlaySnd
trigger1 = time = 0
value = 170,0

[State 170, snd]
type = PlaySnd
trigger1 = time = 80
value = 170,1



[Statedef 180]
type = S

[State 180,ST ]
type = ChangeState
trigger1 = time = 0 && win && !matchover
value = 181

[State 180,ST ]
type = ChangeState
trigger1 = time = 0 && win && matchover
value = 182

[Statedef 181]
type = S
anim = 181

[State 180,sn ]
type = PlaySnd
trigger1 = animelem = 3
value = 180,0

[State 180,AS ]
type = AssertSpecial
trigger1 = time < 300
flag = roundnotover

[Statedef 1800]
type = S
anim = 8888

[State 1801,asp ]
type = AssertSpecial
trigger1 = 1
flag = nomusic

[State 1800,sn ]
type = PlaySnd
trigger1 = time = 0
value = 18000,0
volume = 100

[State 1800,st ]
type = ChangeState
trigger1 = time = 300
value = 1801

[Statedef 1801]
type = S
anim = 8888

[State 1801,asp ]
type = AssertSpecial
trigger1 = 1
flag = nobardisplay
flag2 = nomusic

[State 1801,ex ]
type = Explod
trigger1 = time = 20
postype = left
anim = 9500
ID = 9500
pos = -200,0
vel = 17,0
accel = -.713,0
sprpriority = 3
ontop = 1
ownpal = 1
pausemovetime = 999
removetime = 20

[State 1801,textbox ]
type = Helper
trigger1 = time = 40
helpertype = normal
name = "text box"
ID = 1800
pos = 160,200-screenpos Y
postype = left
stateno = 1802
ownpal = 1
pausemovetime = 1800

[State 1801,ex ]
type = Explod
trigger1 = time = 40
postype = left
anim = 9500
ID = 9500
pos = 0,0
vel = .06,0
sprpriority = 1
ontop = 1
ownpal = 1
pausemovetime = 999
accel = -.00007,0


[State 1801,ev ]
type = EnvColor
trigger1 = time = 0
time = 3

[State 1801,asp ]
type = AssertSpecial
trigger1 = time < 720
flag = roundnotover
flag2 = noFG

[State 1801,va ]
type = VarAdd
trigger1 = time >= 0 && var(1) < 250
v =  1
value = 15

[State 1800, ]
type = Pause
trigger1 = time = 0
time = 999
movetime = 999

[State 1800,pf ]
type = BGPalFX
trigger1 = time = [0,50]
time = 2
color = 256-var(1)

[State 1800,pf ]
type = BGPalFX
trigger1 = time > 50
time = 2
color = 1

[Statedef 1802]
type = S
anim = 8888
sprpriority = 4

[State 1801,ex ]
type = Explod
trigger1 = time = 0
postype = p1
anim = 9501
ID = 9500
pos = 0,0
sprpriority = 5
ontop = 1
ownpal = 1
pausemovetime = 999
scale = .1*var(1),.1*var(1)

[State 1801,ex ]
type = Explod
trigger1 = time = 0
postype = p1
anim = 9502
ID = 9500
pos = 0,0
sprpriority = 5
ontop = 1
ownpal = 1
pausemovetime = 999
scale = .1*var(1),.1*var(1)

[State 1801,ex ]
type = Explod
trigger1 = time = 0
postype = p1
anim = 9503+random%5
ID = 9500
pos = 0,0
sprpriority = 5
ontop = 1
ownpal = 1
pausemovetime = 999
scale = .1*var(1),.1*var(1)


[State 180,va ]
type = VarAdd
trigger1 = timemod = 1,0
v =  1
value = 1

[State 1802,me ]
type = ModifyExplod
trigger1 = time < 10
ID = 9500
scale = .1*var(1),.1*var(1)

[Statedef 182]
type = S
anim = 182
sprpriority = 2

[State 180,sn ]
type = PlaySnd
trigger1 = animelem = 7
value = 180,1
volume = 255

[State 182,sn ]
type = PlaySnd
trigger1 = animelem = 2,4
trigger2 = animelem = 7
value = 1,3

[State 182,ex ]
type = Explod
trigger1 = animelem = 2,4
anim = 183
ID = 1800
pos = 13,-93
ID = 1811
scale = 1,1
bindtime = -1
sprpriority = 1
ownpal = 1
removetime = 126

[State 182,ex ]
type = Explod
trigger1 = animelem = 7
anim = 183
ID = 1800
pos = 0,-93
facing = -1
scale = 1,1
bindtime = -1
sprpriority = 1
ownpal = 1
removetime = 100

[State 182,ex ]
type = Explod
trigger1 = time = 90
anim = 70011
ID = 1800
pos = 0,-83
facing = 1
scale = 1,1
bindtime = -1
sprpriority = 4
ownpal = 1
removetime = -2

[State 182,sn ]
type = PlaySnd
trigger1 = time = 90
value = 0,1

[State 182,sn ]
type = PlaySnd
trigger1 = time = 161
value = 0,6


[State 182,ex ]
type = Explod
trigger1 = animelem = 7,100
anim = 184
ID = 1800
pos = 0,-93
facing = -1
scale = 1,1
bindtime = -1
sprpriority = 1
ownpal = 1
removetime = -2

[State 182,ex ]
type = Explod
trigger1 = animelem = 7,100
anim = 184
ID = 1800
pos = 13,-93
ID = 1811
scale = 1,1
bindtime = -1
sprpriority = 1
ownpal = 1
removetime = -2

[State 182,ex ]
type = Explod
trigger1 = animelem = 7,108
anim = 1842
ID = 1800
pos = 4,-93
ID = 1812
facing = -1
scale = 1,1
bindtime = -1
sprpriority = 3
ownpal = 1
removetime = -2

[State 182,ex ]
type = Explod
trigger1 = animelem = 7,108
anim = 1842
ID = 1800
pos = 15,-93
facing = 1
scale = 1,1
bindtime = -1
sprpriority = 3
ownpal = 1
removetime = -2

[State 182,ex ]
type = Explod
trigger1 = animelem = 8
anim = 1823
pos = 0,0
facing = 1
scale = 1,1
bindtime = 1
sprpriority = 3
ownpal = 1
accel = -.1,.1
removetime = -2

[State 182,ex ]
type = Explod
trigger1 = animelem = 8
anim = 1824
pos = 0,0
facing = 1
scale = 1,1
bindtime = 1
sprpriority = 3
ownpal = 1
accel = .1,-.1
removetime = -2

[State 182,ex ]
type = Explod
trigger1 = time = 161
anim = 1825
ID = 18012
pos = 15,-90
scale = 1,1
bindtime = -1
sprpriority = 4
ownpal = 1
removetime = -2

[State 182,ex ]
type = Explod
trigger1 = time = 161
anim = 1826
ID = 18012
pos = 15,-90
accel = 0,.1
scale = 1,1
bindtime = -1
sprpriority = 4
ownpal = 1
removetime = -2


[State 182,me ]
type = null; ModifyExplod
trigger1 = time = [151,161]
ID = 1800
scale = .1*(161-time),.1*(161-time)

[State 180,AS ]
type = AssertSpecial
trigger1 = time < 300
flag = roundnotover


;INTRO
[Statedef 190]
type = S
anim = 1900

[State 190,hp ]
type = Helper
trigger1 = time = 0
helpertype = normal
name = "Sachielenter"
ID = 190
pos = 0,-240
stateno = 1901
ownpal = 1

[State 190,as ]
type = AssertSpecial
trigger1 = time >= 0
flag = intro

[State 1901,ex ]
type = Explod
trigger1 = time > 395 && timemod = 1,0
anim = 7010
ID = 190
pos = 0,-60
vel = 0,-5
random = 30,70
scale = 2,2
sprpriority = 6
ownpal = 1

[State 1901,ex ]
type = Explod
trigger1 = time > 395 && timemod = 1,0
anim = 7010
ID = 190
pos = 0,-60
vel = 0,-3
random = 30,70
scale = .5,.5
sprpriority = 6
ownpal = 1

[State 1901,ex ]
type = Explod
trigger1 = time > 395 && timemod = 1,0
anim = 7010
ID = 190
pos = 0,-60
vel = 0,-4
random = 30,70
scale = 1,1
sprpriority = 6
ownpal = 1

[State 1901,ex ]
type = Explod
trigger1 = time > 395 && timemod = 1,0
anim = 7010
ID = 190
pos = 0,0
vel = 0,-4
random = 60,0
scale = 1.5,1.5
sprpriority = 6
ownpal = 1


[State 190,an ]
type = ChangeAnim
trigger1 = numhelper(190) > 0 && helper(190), pos Y >= 0
value = 1901

[State 190, ]
type = PlaySnd
trigger1 = numhelper(190) > 0 && helper(190), pos Y >= 0
value = 190,0

[State 190,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 0


[Statedef 1901]
type = S
physics = N
anim = 190
velset = 0,.6

[State 1901,pf ]
type = PalFX
trigger1 = timemod = 40,0
time = 30
add = 250,250,250
sinadd = -250,-250,-250, 120
invertall = 0

[State 1901,ex ]
type = Explod
trigger1 = timemod = 2,0
anim = 7010
ID = 190
pos = 0,-70
vel = 0,-.4
random = 30,70
scale = 1.5, 1.5
sprpriority = 6
ownpal = 1

[State 1901,ex ]
type = Explod
trigger1 = timemod = 2,0
anim = 7010
ID = 190
pos = 0,-110
vel = 0,-.8
random = 30,30
scale = .5, .5
sprpriority = 6
ownpal = 1

[State 1901,ex ]
type = Explod
trigger1 = timemod = 2,0
anim = 7010
ID = 190
pos = -5,-50
vel = 0,-.6
random = 30,50
scale = 1.2, 1.2
sprpriority = 6
ownpal = 1

[State 1901,ex ]
type = Explod
trigger1 = timemod = 3,0
anim = 7010
ID = 190
pos = -5,-60
vel = 0,-.8
random = 30,70
scale = 2,2
sprpriority = 6
ownpal = 1

[State 1901,ex ]
type = Explod
trigger1 = timemod = 4,0
anim = 7010
ID = 190
pos = -10,-10
vel = 0,-.8
random = 20,10
scale = 2,2
sprpriority = 6
ownpal = 1

[State 190,ds ]
type = DestroySelf
trigger1 = pos Y >= 0




[Statedef 195]
type = S
anim = 195
ctrl = 0
velset = 0,0

[State 240,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 195,0
channel = 0

[State 195,ctr ]
type = CtrlSet
trigger1 = time = 70
value = 1



[State 240,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1

;--------------------------------------------
;STANDING ATTACKS
[Statedef 200]
type = S
movetype = A 
physics = S
velset = 0,0
anim = 200
ctrl = 0
juggle = 11

[State 240,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 200,0

[State 240,ht ]
type = HitDef
trigger1 = time = 0
attr = S,NA
hitflag = MAF
guardflag = MA
animtype = Light
damage = 48,0
pausetime = 10,12
sparkno = 0
sparkxy = 0,-78
hitsound = 5,1
guardsound = 6,0
ground.type = high
air.type = low
ground.slidetime = 9
ground.hittime = 9
ground.velocity = -7,0
air.velocity = -3.5,-6
ground.cornerpush.veloff = -14
air.cornerpush.veloff = -6
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30

[State 240,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1

[Statedef 205]
type = S
movetype = A 
physics = S
velset = 0,0
anim = 205
ctrl = 0
juggle = 11

[State 240,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 200,0

[State 240,ht ]
type = HitDef
trigger1 = time = 0
attr = S,NA
hitflag = MAF
guardflag = MA
animtype = Light
damage = 48,0
pausetime = 10,12
sparkno = 0
sparkxy = 0,-88
hitsound = 5,1
guardsound = 6,0
ground.type = high
air.type = low
ground.slidetime = 9
ground.hittime = 9
ground.velocity = -7,0
air.velocity = -3.5,-6
ground.cornerpush.veloff = -14
air.cornerpush.veloff = -6
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30

[State 240,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1

[Statedef 210]
type = S
movetype = A 
physics = S
velset = 0,0
anim = 210
ctrl = 0
juggle = 15

[State 210,sn ]
type = PlaySnd
trigger1 = animelem = 2 && random < 500
value = 2,8
channel = 0

[State 210,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 200,0


[State 210,ht ]
type = HitDef
trigger1 = animelem = 3
attr = S,NA
hitflag = MAF
guardflag = MA
animtype = Medium
priority = 6
damage = 92,0
pausetime = 6,8
sparkno = 1
sparkxy = 0,-85
hitsound = 5,2
guardsound = 6,0
ground.type = High
air.type = low
ground.slidetime = 17
ground.hittime = 17
ground.velocity = -6,0
air.velocity = -3.5,-6
ground.cornerpush.veloff = -5
air.cornerpush.veloff = -6
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30

[State 210,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1

[Statedef 220]
type = S
movetype = A 
physics = S
velset = 0,0
anim = 220
ctrl = 0
juggle = 10

[State 210,sn ]
type = PlaySnd
trigger1 = animelem = 2 
value = 2,11
channel = 0


[State 230,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 200,1

[State 220,ex ]
type = Explod
trigger1 = animelem = 3
anim = 221
pos = 45,-60
bindtime = -1
scale = 1, 1
sprpriority = 4
removeongethit = 1

[State 220,ex ]
type = Explod
trigger1 = animelem = 7
anim = 222
pos = 0,0
bindtime = -1
scale = 1, 1
sprpriority = 4
removeongethit = 1

[State 1000,ex ]
type = Explod
trigger1 = movehit = 1
anim = F12
pos = ceil(p2dist X),-60
bindtime = 1
ontop = 1
sprpriority = 5
ownpal = 1
persistent = 0
ignorehitpause = 1

[State 1000,ex ]
type = Explod
trigger1 = movehit = 1
anim = F11
pos = ceil(p2dist X),-60
facing = -1
bindtime = 1
ontop = 1
sprpriority = 5
ownpal = 1
persistent = 0
ignorehitpause = 1

[State 240,ht ]
type = HitDef
trigger1 = animelem = 3
attr = S,NA
hitflag = MAF
guardflag = MA
animtype = Light
damage = 48,0
pausetime = 8,10
sparkno = S7000+random%3
sparkxy = 0,-58
hitsound = S0,4
guardsound = 6,0
ground.type = Low
air.type = low
ground.slidetime = 12
ground.hittime = 12
ground.velocity = -4,0
air.velocity = -3.5,-6
ground.cornerpush.veloff = -14
air.cornerpush.veloff = -6
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30

[State 240,ht ]
type = HitDef
trigger1 = animelem = 6
attr = S,NA
hitflag = MAF
guardflag = HA
animtype = Medium
damage = 68,0
pausetime = 8,10
sparkno = S7000+random%3
sparkxy = 0,-78
hitsound = S0,5
guardsound = 6,0
ground.type = High
air.type = low
ground.slidetime = 12
ground.hittime = 25
ground.velocity = -7,0
air.velocity = -3.5,15
ground.cornerpush.veloff = -14
air.cornerpush.veloff = -6
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30
air.fall = 1
fall.recover = 0

[State 230,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1

[Statedef 230]
type = S
movetype = A 
physics = S
velset = 0,0
anim = 230
ctrl = 0
juggle = 11

[State 240,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 200,1

[State 240,ht ]
type = HitDef
trigger1 = time = 0
attr = S,NA
hitflag = MAF
guardflag = MA
animtype = Light
damage = 48,0
pausetime = 10,12
sparkno = 0
sparkxy = 0,-78
hitsound = 5,1
guardsound = 6,0
ground.type = high
air.type = low
ground.slidetime = 9
ground.hittime = 9
ground.velocity = -7,0
air.velocity = -3.5,-6
ground.cornerpush.veloff = -14
air.cornerpush.veloff = -6
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30

[State 240,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1

[Statedef 235]
type = S
movetype = A 
physics = S
velset = 0,0
anim = 235
ctrl = 0
juggle = 11

[State 240,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 200,1

[State 240,ht ]
type = HitDef
trigger1 = time = 0
attr = S,NA
hitflag = MAF
guardflag = MA
animtype = Light
damage = 52,0
pausetime = 10,12
sparkno = 0
sparkxy = 0,-58
hitsound = 5,1
guardsound = 6,0
ground.type = low
air.type = low
ground.slidetime = 9
ground.hittime = 9
ground.velocity = -7,0
air.velocity = -3.5,-6
ground.cornerpush.veloff = -14
air.cornerpush.veloff = -6
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30

[State 240,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1

[Statedef 240]
type = S
movetype = A 
physics = S
velset = 0,0
anim = 240
ctrl = 0
juggle = 15

[State 210,sn ]
type = PlaySnd
trigger1 = animelem = 2 && random < 500
value = 2,9

[State 240,sn ]
type = PlaySnd
trigger1 = animelem = 4
value = 200,1


[State 240,ht ]
type = HitDef
trigger1 = time = 0
attr = S,NA
hitflag = MAF
guardflag = MA
animtype = Medium
priority = 6
damage = 98,0
pausetime = 10,12
sparkno = 1
sparkxy = 0,-78
hitsound = 5,2
guardsound = 6,0
ground.type = High
air.type = low
ground.slidetime = 17
ground.hittime = 17
ground.velocity = -12,0
air.velocity = -3.5,-6
ground.cornerpush.veloff = -14
air.cornerpush.veloff = -6
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30

[State 240,pa1 ]
type = PosAdd
trigger1 = animelem = 3
x = 16

[State 240,pa1 ]
type = PosAdd
trigger1 = animelem = 3
x = 4

[State 240,pa1 ]
type = PosAdd
trigger1 = animelem = 4
x = 15

[State 240,pa1 ]
type = PosAdd
trigger1 = animelem = 5
x = 5

[State 240,pa1 ]
type = PosAdd
trigger1 = animelem = 8
x = 10

[State 240,pa1 ]
type = PosAdd
trigger1 = animelem = 9
x = 10

[State 240,pa1 ]
type = PosAdd
trigger1 = animelem = 10
x = 5

[State 240,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1


[Statedef 245]
type = S
movetype = A 
physics = S
velset = 0,0
anim = 245
ctrl = 0
juggle = 15

[State 210,sn ]
type = PlaySnd
trigger1 = animelem = 2 && random < 500
value = 2,9

[State 240,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 200,1

[State 240,ht ]
type = HitDef
trigger1 = time = 0
attr = S,NA
hitflag = MAF
guardflag = MA
animtype = Medium
priority = 6
damage = 102,0
pausetime = 10,12
sparkno = 1
sparkxy = 0,-78
hitsound = 5,2
guardsound = 6,0
ground.type = High
air.type = low
ground.slidetime = 17
ground.hittime = 17
ground.velocity = -12,0
air.velocity = -3.5,-6
ground.cornerpush.veloff = -14
air.cornerpush.veloff = -6
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30

[State 240,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1


;CROUCHING ATTACKS
[Statedef 400]
type = C
movetype = A 
physics = C
velset = 0,0
anim = 400
ctrl = 0
juggle = 11

[State 240,sn ]
type = PlaySnd
trigger1 = time = 0
value = 200,0

[State 240,ht ]
type = HitDef
trigger1 = time = 0
attr = C,NA
hitflag = MAD
guardflag = L
animtype = Light
damage = 49,0
pausetime = 10,12
sparkno = 1
sparkxy = 0,-40
hitsound = 5,1
guardsound = 6,0
ground.type = Low
air.type = low
ground.slidetime = 9
ground.hittime = 9
ground.velocity = -7,0
air.velocity = -3.5,-6
ground.cornerpush.veloff = -14
air.cornerpush.veloff = -6
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30

[State 240,st ]
type = ChangeState
trigger1 = animtime = 0
value = 11
ctrl = 1

[Statedef 250]
type = S
movetype = A
physics = S
juggle = 15*(prevstateno != [150,152])
velset = 0,0
anim = 250
ctrl = 0
sprpriority = 3

[State 210,sn ]
type = PlaySnd
trigger1 = animelem = 2 && random < 500
value = 2,12

[State 250,ex ]
type = Explod
trigger1 = prevstateno = 150 || prevstateno = 152
trigger1 = time = 0
anim = 710
ID = 700
pos = 0,-65
pausemovetime = 6
ontop = 1
ownpal = 1
persistent = 0

[State 250,p]
type = Pause
trigger1 = prevstateno = 150 || prevstateno = 152
trigger1 = time = 0
time = 5
movetime = 5
ignorehitpause = 1

[State 250, pow]
type = PowerAdd
trigger1 = prevstateno = 150 || prevstateno = 152
trigger1 = time = 0
value = -1000   

[State 250, nhb]
type = NotHitBy
trigger1 = prevstateno = 150 || prevstateno = 152
value = SCA,AA,AT,AP
time = 10


[State 250,hit ]
type = HitDef
trigger1 = animelem = 4
attr = S,NA
hitflag = MAF
guardflag = MA
animtype = Hard
priority = 7
damage = 140,0
pausetime = 10,12
sparkno = S7000
guard.sparkno = 40
sparkxy = 0,-80
hitsound = S0,5
guardsound = 6,0
ground.type = high
ground.slidetime = 20
ground.hittime = 20
ground.velocity = -2,-12
guard.velocity = -12
air.velocity = -2,-12
ground.cornerpush.veloff = -12
air.cornerpush.veloff = -6
fall = 1
fall.recover = 0
yaccel = .70
getpower = 40*ifelse(var(0) > 0,2,1),5
givepower = 10,30
down.bounce = 1

[State 1000,ex ]
type = Explod
trigger1 = movehit = 1
anim = F12
pos = ceil(p2dist X),-86
bindtime = 1
ontop = 1
sprpriority = 5
ownpal = 1
ignorehitpause = 1
persistent = 0

[State 1000,ex ]
type = Explod
trigger1 = movehit = 1
anim = F11
pos = ceil(p2dist X),-83
facing = -1
bindtime = 1
ontop = 1
sprpriority = 5
ownpal = 1
ignorehitpause = 1
persistent = 0

[State 210,ex ]
type = Explod
trigger1 = animelem = 3
anim = 211
sprpriority = 6
ownpal = 1
removeongethit = 1

[State 250,sn ]
type = PlaySnd
trigger1 = AnimElem = 4
value = 1,6

[State 250,sn2 ]
type = PlaySnd
trigger1 = AnimElem = 4
value = 250,0

[State 250,end ]
type = ChangeState
trigger1 = AnimTime = 0
value = 0
ctrl = 1

[Statedef 410]
type = C
movetype = A 
physics = C
velset = 0,0
anim = 410
ctrl = 0
juggle = 5

[State 210,sn ]
type = PlaySnd
trigger1 = animelem = 4
value = 200,0

[State 210,ht ]
type = HitDef
trigger1 = time = 0
attr = C,NA
hitflag = MAFD
guardflag = L
animtype = Hard
priority = 6
damage = 112,0
pausetime = 10,12
sparkno = 2
sparkxy = 10,-60
hitsound = 5,3
guardsound = 6,0
ground.type = Trip
air.type = low
ground.slidetime = 17
ground.hittime = 17
ground.velocity = -5,-3
air.velocity = -3.5,-6
ground.cornerpush.veloff = -14
air.cornerpush.veloff = -6
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30
fall = 1
fall.recover = 0

[State 410,sn ]
type = PlaySnd
trigger1 = animelem = 4
value = F5,2

[State 410, ]
type = EnvShake
trigger1 = animelem = 5
time = 3    

[State 410,ex ]
type = Explod
trigger1 = animelem = 5
anim = F61
pos = 80,-4

[State 410,ex ]
type = Explod
trigger1 = animelem = 5
anim = 411
pos = 79,-4
sprpriority = 5
scale = 1,1
removeongethit = 1

[State 210,st ]
type = ChangeState
trigger1 = animtime = 0
value = 11  
ctrl = 1


[Statedef 430]
type = C
movetype = A 
physics = C
velset = 0,0
anim = 430
ctrl = 0
juggle = 11

[State 240,sn ]
type = PlaySnd
trigger1 = time = 0
value = 200,1

[State 240,ht ]
type = HitDef
trigger1 = time = 0
attr = C,NA
hitflag = MAD
guardflag = L
animtype = Light
damage = 44,0
pausetime = 10,12
sparkno = 0
sparkxy = 0,-4
hitsound = 5,1
guardsound = 6,0
ground.type = Low
air.type = low
ground.slidetime = 9
ground.hittime = 9
ground.velocity = -7,0
air.velocity = -3.5,-6
ground.cornerpush.veloff = -14
air.cornerpush.veloff = -6
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30

[State 240,st ]
type = ChangeState
trigger1 = animtime = 0
value = 11
ctrl = 1

[Statedef 440]
type = C
movetype = A 
physics = C
velset = 0,0
anim = 440
ctrl = 0
juggle = 15

[State 210,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 200,1

[State 210,ht ]
type = HitDef
trigger1 = time = 0
attr = C,NA
hitflag = MAD
guardflag = L
animtype = hard
priority = 6
damage = 100,0
pausetime = 10,12
sparkno = 1
sparkxy = 10,-40
hitsound = 5,3
guardsound = 6,0
ground.type = trip
air.type = trip
ground.slidetime = 17
ground.hittime = 17
ground.velocity = -5,-3
air.velocity = -3.5,-3
ground.cornerpush.veloff = -14
air.cornerpush.veloff = -6
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30
fall = 1
fall.recover = 0


[State 210,st ]
type = ChangeState
trigger1 = animtime = 0
value = 11  
ctrl = 1

;JUMPING ATTACKS
[Statedef 600]
type = A
movetype = A 
physics = A
anim = 610
ctrl = 0
juggle = 10

[State 240,sn ]
type = PlaySnd
trigger1 = time = 0
value = 200,0

[State 240,ht ]
type = HitDef
trigger1 = time = 0
attr = S,NA
hitflag = MAF
guardflag = MA
animtype = Medium
priority = 6
damage = 55,0
pausetime = 10,12
sparkno = 1
sparkxy = 0,-75
hitsound = 5,0
guardsound = 6,0
ground.type = High
air.type = low
ground.slidetime = 9
ground.hittime = 10
ground.velocity = -7,0
air.velocity = -3.5,-6
ground.cornerpush.veloff = -6
air.cornerpush.veloff = -6
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30

[Statedef 610]
type = A
movetype = A 
physics = A
anim = 600
ctrl = 0
juggle = 10

[State 610,ex ]
type = Explod
trigger1 = time = 0
anim = 601
id = 600
pos = 0,0
bindtime = -1
sprpriority = 4
ownpal = 1
removeongethit = 1

[State 240,sn ]
type = PlaySnd
trigger1 = time = 0
value = 1,3

[State 240,ht ]
type = HitDef
trigger1 = time = 0
attr = S,NA
hitflag = MAF
guardflag = MA
animtype = Medium
damage = 100,0
pausetime = 10,12
sparkno = S7002
sparkxy = 0,-38
hitsound = S0,2
guardsound = 6,0
ground.type = High
air.type = low
ground.slidetime = 18
ground.hittime = 17
ground.velocity = -12,0
air.velocity = -3.5,-6
ground.cornerpush.veloff = -10
air.cornerpush.veloff = -10
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30

[Statedef 630]
type = A
movetype = A 
physics = A
anim = 630
ctrl = 0
juggle = 10

[State 240,sn ]
type = PlaySnd
trigger1 = time = 0
value = 200,1

[State 240,ht ]
type = HitDef
trigger1 = time = 0
attr = S,NA
hitflag = MAF
guardflag = MA
animtype = Medium
priority = 6
damage = 52,0
pausetime = 10,12
sparkno = 1
sparkxy = 0,-78
hitsound = 5,2
guardsound = 6,0
ground.type = High
air.type = low
ground.slidetime = 14
ground.hittime = 15
ground.velocity = -7,0
air.velocity = -3.5,-6
ground.cornerpush.veloff = -10
air.cornerpush.veloff = -10
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30

[Statedef 640]
type = A
movetype = A 
physics = A
anim = 640
ctrl = 0
juggle = 10

[State 240,sn ]
type = PlaySnd
trigger1 = time = 0
value = 200,1

[State 240,ht ]
type = HitDef
trigger1 = time = 0
attr = S,NA
hitflag = MAF
guardflag = MA
animtype = Medium
priority = 6
damage = 102,0
pausetime = 10,12
sparkno = 1
sparkxy = 0,-78
hitsound = 5,2
guardsound = 6,0
ground.type = High
air.type = low
ground.slidetime = 14
ground.hittime = 15
ground.velocity = -12,0
air.velocity = -3.5,-6
ground.cornerpush.veloff = -10
air.cornerpush.veloff = -10
getpower = 30*ifelse(var(0) > 0,2,1),5
givepower = 5,30

[Statedef 650]
type = A
movetype = A 
physics = A
anim = 650
ctrl = 0
juggle = 15
sprpriority = 2 

[State 210,sn ]
type = PlaySnd
trigger1 = animelem = 2 && random < 500
value = 2,9

[State 610,ex ]
type = Explod
trigger1 = animelem = 2
anim = 222
id = 651
pos = -20,0
bindtime = -1
sprpriority = 4
scale = 1.3,1.3
ownpal = 1
removeongethit = 1

[State 610,ex ]
type = Explod
trigger1 = time = 0
anim = 651
id = 650
pos = 0,0
bindtime = -1
sprpriority = 4
ownpal = 1
removeongethit = 1


[State 610,re ]
type = RemoveExplod
trigger1 = animelem = 3
ID = 650


[State 610,ex ]
type = Explod
trigger1 = animelem = 3
anim = 652
id = 651
pos = 0,0
bindtime = -1
sprpriority = 4
ownpal = 1
removeongethit = 1


[State 240,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 1,6

[State 240,ht ]
type = HitDef
trigger1 = time = 0
attr = S,NA
hitflag = MAF
guardflag = MA
animtype = Medium
priority = 7
damage = 170,0
pausetime = 8,10
sparkno = S7003
guard.sparkno = 40
sparkxy = 0,-30
hitsound = S0,5
guardsound = 6,0
ground.type = high
ground.slidetime = 20
ground.hittime = 20
ground.velocity = -2,-12
guard.velocity = -12
air.velocity = -2,-12
ground.cornerpush.veloff = -12
air.cornerpush.veloff = -6
fall = 1
fall.recover = 0
yaccel = .70
getpower = 40*ifelse(var(0) > 0,2,1),5
givepower = 10,30
down.bounce = 1

;FORWARD ROLL
[Statedef 700]
type    = S
movetype = I
physics = N
ctrl = 0
anim = 701
sprpriority = -2

[State 701,sn ]
type = PlaySnd
trigger1 = AnimElem = 3
value = 700,0


[State 701,ex ]
type = Explod
trigger1 = prevstateno = 150 || prevstateno = 152
trigger1 = time = 0
anim = 710
ID = 700
pos = 0,-65
pausemovetime = 6
ontop = 1
ownpal = 1
persistent = 0

[State 701,p]
type = Pause
trigger1 = prevstateno = 150 || prevstateno = 152
trigger1 = time = 0
time = 6
movetime = 6
ignorehitpause = 1

[State 701, pow]
type = PowerAdd
trigger1 = prevstateno = 150 || prevstateno = 152
trigger1 = time = 0
value = -1000   

[State 701,nohit]
type = NotHitBy
trigger1 = time > 0
value = SCA,NA,SA,HA,NP,SP,HP
time = 2

[State 701,vel ]
type = VelSet
trigger1 = AnimElem = 3
x = 5.5

[State 701,velB ]
type = VelAdd
trigger1 = time = [16,24]
x = -.1

[State 701,velC ]
type = VelSet
trigger1 = AnimElem = 9
x = 0

[State 701, asp]
type = AssertSpecial
trigger1 = time >= 0
flag = noshadow 

[State 701,end ]
type = ChangeState
trigger1 = AnimTime = 0
value = 0
ctrl = 1

[State 701, no push]
type = PlayerPush
trigger1 = Time > 0
value = 0


;BACKWARD ROLL
[Statedef 701]
type    = S
movetype = I
physics = N
ctrl = 0
anim = 701
sprpriority = -2

[State 701,sn ]
type = PlaySnd
trigger1 = AnimElem = 3
value = 700,0

[State 701,ex ]
type = Explod
trigger1 = prevstateno = 150 || prevstateno = 152
trigger1 = time = 0
anim = 710
ID = 700
pos = 0,-65
pausemovetime = 6
ontop = 1
ownpal = 1
persistent = 0

[State 701,p]
type = Pause
trigger1 = prevstateno = 150 || prevstateno = 152
trigger1 = time = 0
time = 6
movetime = 6
ignorehitpause = 1

[State 701, pow]
type = PowerAdd
trigger1 = prevstateno = 150 || prevstateno = 152
trigger1 = time = 0
value = -1000   

[State 701,vel ]
type = VelSet
trigger1 = AnimElem = 3
x = -5.5

[State 701,velB ]
type = VelAdd
trigger1 = time = [16,24]
x = .1

[State 701,velC ]
type = VelSet
trigger1 = AnimElem = 9
x = 0

[State 701, asp]
type = AssertSpecial
trigger1 = time >= 0
flag = noshadow 


[State 701,nohit]
type = NotHitBy
trigger1 = Time > 0
value = SCA,NA,SA,HA,NP,SP,HP
time = 2

[State 701,end ]
type = ChangeState
trigger1 = AnimTime = 0
value = 0
ctrl = 1

[State 701, no push]
type = PlayerPush
trigger1 = Time > 0
value = 0

;----------
[Statedef 750]
type    = S
movetype = I
physics = S
ctrl = 0
anim = 750
sprpriority = 3
velset = 0

[State 750,vo ]
type = PlaySnd
trigger1 = time = 5
value = 750,1

[State 750, p]
type = PowerAdd
trigger1 = (AnimElem = 4,1) && (timemod = 3,0)
value = 30*ifelse(var(0) > 0,2,1)

[State 750,sn ]
type = PlaySnd
trigger1 = time = 0
value = 750,0

[State 750,ex ]
type = Explod
trigger1 = (animelem = 4,>= 0) && timemod = 5,0
anim = 752
ID = 750
bindtime = -1
sprpriority = 4
ownpal = 1
removeongethit = 1


[State 750,hp ]
type = Helper
trigger1 = animelem = 4
name = "effetti sparaflashosi"
ID = 750
pos = 0,-133
stateno = 752
ownpal = 1

[State 750,c ]
type = ChangeState
trigger1 = command !="holdpowerup"
trigger2 = power = 3000
value = 751

[Statedef 751]
type    = S
movetype = I
physics = S
ctrl = 0
anim = 751

[State 750,vo ]
type = PlaySnd
trigger1 = time = 0
trigger1 = power = 3000
value = 750,2

[State 751,p ]
type = PalFX
trigger1 = time = 0
trigger1 = power = 3000
time = 3
add = 255,255,255
sinadd = 0,0,0,10

[State 751,c ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1

[Statedef 752]
type    = S
movetype = I
physics = N
ctrl = 0
anim = 8888
velset = 0,0

[State 752,bin ]
type = BindToParent
trigger1 = time > 0
pos = 0,-133

[State 752,ds ]
type = DestroySelf
trigger1 = parent,stateno != 750
ignorehitpause = 1

[State 752,vs ]
type = Varrandom
trigger1 = timemod = 2,0
v =  1
range = -130,130

[State 752,vs ]
type = Varrandom
trigger1 = timemod = 2,0
v =  2
range = -130,130

[State 752,vs ]
type = Varrandom
trigger1 = timemod = 2,0
v =  3
range = 5,15

[State 752,effettino ]
type = Helper
trigger1 = (timemod = 2,0) && (parent,stateno = 750)
name = "effettucolo"
ID = 752
pos = var(1),var(2)
stateno = 7521
ownpal = 1
size.xscale = .1*(var(3))
size.yscale = .1*(var(3))

[Statedef 7521]
type    = S
movetype = I
physics = N
ctrl = 0
anim = 70101
sprpriority = 5

[State 11050,vs ]
type = VarSet
trigger1 = time = 0
v =  1
value = -250

[State 7521,vs ]
type = VelSet
trigger1 = time > 0
x = (parentdist X)/10
y = (parentdist Y)/10

[State 752,ds ]
type = DestroySelf
trigger1 = time = 22
trigger2 = root,stateno !=750
ignorehitpause = 1


;Throw Y
[Statedef 800]
type = S
movetype = A
physics = S
velset = 0,0
anim = 800
ctrl = 0
juggle = 15

[State 800,ht ]
type = HitDef
trigger1 = time = 0
attr = S,NT
hitflag = MAF
priority = 4,miss
guardflag = 
sparkno = -1
sparkxy = -30,-70
snap = 50,0
hitsound = -1
p1stateno = 801
p2facing = 1
p2stateno = 810
fall = 1
ground.velocity = 6,5
getpower = 0,0
givepower = 0,0

[State 800,c ]
type = ChangeState
trigger1 = AnimTime = 0
value = 0
ctrl = 1

[Statedef 801]
type = S
movetype = I
physics = S
velset = 0,0
anim = 801
ctrl = 0

[State 801,w ]
type = Width
trigger1 = time = 0
edge = 80,20

[State 801,tb ]
type = TargetBind
trigger1 = time = 0
pos = 30,0

[State 801,PP ]
type = PlayerPush
trigger1 = 1
value = 0


[State 800,c ]
type = ChangeState
trigger1 = AnimTime = 0
value = 802

[Statedef 802]
type = A
movetype = A
physics = N
velset = 2.5,-10
anim = 802
ctrl = 0
juggle = 15

[State 802,va ]
type = VelAdd
trigger1 = animelem = 9,<0
y = .5

[State 802,va ]
type = VelSet
trigger1 = animelem = 9
y = 0
x = 0

[State 802,tla ]
type = TargetLifeAdd
trigger1 = animelem = 9
value = -90

[State 802,ex ]
type = Explod
trigger1 = animelem = 9
anim = F2
pos = -27,-7
ontop = 1
ownpal = 1

[State 802,sm ]
type = PlaySnd
trigger1 = animelem = 9
value = F5,2

[State 802, ts]
type = TargetState
trigger1 = animelem = 9
value = 811


[State 801,PP ]
type = PlayerPush
trigger1 = 1
value = 0

[State 800,c ]
type = ChangeState
trigger1 = AnimTime = 0
value = 803


[Statedef 803]
type = A
movetype = I
physics = A
velset = 2.5,-3
anim = 803
ctrl = 0
juggle = 15

[State 800,c ]
type = null
trigger1 = AnimTime = 0

[Statedef 810]
type = S
movetype = H
velset = 0,0
ctrl = 0

[State 810,ca2 ]
type = ChangeAnim2
trigger1 = time = 0
value = 810

[Statedef 811]
type = A
movetype = H
physics = N
ctrl = 0

[State 810,ca2 ]
type = ChangeAnim2
trigger1 = time = 0
value = 811

[State 811,ss ]
type = SelfState
trigger1 = animtime = 0
value = 5000



[Statedef 850]
type = S
movetype = A
physics = S
velset = 0,0
anim = 800
ctrl = 0
juggle = 15

[State 800,ht ]
type = HitDef
trigger1 = time = 0
attr = S,NT
hitflag = MAF
priority = 4,miss
guardflag = 
sparkno = -1
sparkxy = -30,-70
snap = 50,0
hitsound = -1
p1stateno = 851
p2facing = 1
p2stateno = 810
fall = 1
ground.velocity = -6,-5
getpower = 0,0
givepower = 0,0

[State 800,c ]
type = ChangeState
trigger1 = AnimTime = 0
value = 0
ctrl = 1


[Statedef 851]
type = S
movetype = A
physics = N
velset = 0,0
anim = 850
ctrl = 0
juggle = 15

[State 802,tla ]
type = TargetLifeAdd
trigger1 = animelem = 5
value = -90

[State 802,ex ]
type = Explod
trigger1 = animelem = 5
anim = F2
pos = 40,-90
ontop = 1
ownpal = 1

[State 802,tb ]
type = TargetBind
trigger1 = time = 0
time = 20
pos = 20,0

[State 802,sm ]
type = PlaySnd
trigger1 = animelem = 2
value = F5,0

[State 802,sm ]
type = PlaySnd
trigger1 = animelem = 5
value = F5,2

[State 802, ts]
type = TargetState
trigger1 = animelem = 5
value = 811

[State 801,PP ]
type = PlayerPush
trigger1 = 1
value = 0

[State 800,c ]
type = ChangeState
trigger1 = AnimTime = 0
value = 0
ctrl = 1

[Statedef 1000]
type = S
movetype = I 
physics = S
poweradd = 100*ifelse(var(0) > 0,2,1)
velset = 0,0
anim = 1000
ctrl = 0
juggle = 15

[State 1000,rd ]
type = ReversalDef
trigger1 = time = 0 
reversal.attr = SCA,NA,SA
numhits = 0
pausetime = 0,0

[State 1000,ex ]
type = Explod
trigger1 = animelem = 2
anim = 1002 
ID = 1002
bindtime = -1
sprpriority = 5
ownpal = 1
removeongethit = 1

[State 1001,st ]
type = StateTypeSet
trigger1 = animelem = 4
movetype = A

[State 1000,va ]
type = VelAdd
trigger1 = animelem = 2,>0
x = 2

[State -2, Jumping Sound]
type = PlaySnd
trigger1 = animtime = 0
value = 40, 1

[State 1000,st ]
type = ChangeState
trigger1 = animtime = 0
value = 1001

[Statedef 1001]
type = A
movetype = A 
physics = A
velset = 0,-5
anim = 1001
juggle = 15
ctrl = 0
sprpriority = 3

[State 1070,nh ]
type = NotHitBy
trigger1 = time >= 0
value = SCA,NA,SA
value2 = SCA, NT, ST, HT


[State 190, ]
type = PlaySnd
trigger1 = time = 0
value = 1000,0
channel = 0

[State 1001,re ]
type = RemoveExplod
trigger1 = time = 0
ID = 1002

[State 1001,help ]
type = Helper
trigger1 = time = 0 && pos Y > -5
name = "many explods 2"
ID = 1000
stateno = 1006
ownpal = 1
pos = 0,0

[State 1001,help ]
type = Helper
trigger1 = time = 0
name = "many explods"
ID = 1000
stateno = 1005
ownpal = 1
pos = 40,-54

[State 1001,sn ]
type = PlaySnd
trigger1 = time = 0
value = 1,5

[State 1001,ps ]
type = PosAdd
trigger1 = time = 0
x = 10

[State 1001,hd ]
type = HitDef
trigger1 = animelem = 1
attr = A,SA
hitflag = MAF
guardflag = MA
animtype = up
air.animtype = diagUp
damage = 150,8
pausetime = 3,5
sparkno = S70021
sparkxy = 0,-80
hitsound = S0,5
guardsound = 6,0
ground.velocity = -4,-8
air.velocity = -4,-8
fall = 1
fall.recover = 0
getpower = 30,0
givepower = 0,30

[State 1000,ex ]
type = Explod
trigger1 = movehit = 1
anim = F12
pos = ceil(p2dist X),-80
bindtime = 1
ontop = 1
sprpriority = 5
ownpal = 1
persistent = 0

[State 1000,ex ]
type = Explod
trigger1 = movehit = 1
anim = F11
pos = ceil(p2dist X),-70
bindtime = 1
ontop = 1
sprpriority = 5
ownpal = 1
persistent = 0

[State 1000,ex ]
type = Explod
trigger1 = time = 0
anim = 1003
bindtime = -1
removetime = 3
sprpriority = 5
ownpal = 1
removeongethit = 1

[State 1000,ex ]
type = Explod
trigger1 = animelem = 1
anim = 1004
bindtime = -1
scale = 1,1
sprpriority = 1
ownpal = 1
removeongethit = 1

[State 1000,ex ]
type = Explod
trigger1 = animelem = 2
anim = 1005
bindtime = -1
scale = 1,1
sprpriority = 1
ownpal = 1
removeongethit = 1

[State 1000,ex ]
type = null;Explod
trigger1 = animelem = 2
trigger2 = animelem = 3
anim = 1006
bindtime = 1
scale = 1,1.2
sprpriority = 1
ownpal = 1
removeongethit = 1

[Statedef 1005]
type = A
movetype = I
physics = N
velset = 12,0
anim = 8888
ctrl = 0

[State 1005,va ]
type = VelAdd
trigger1 = time > 0
x = -1.5
y = -1.5

[State 1005,ex ]
type = Explod
trigger1 = timemod = 1,0
anim = 7010 
pos = 0,0
vel = -.1*(Vel x),-.1*(vel Y)
random = 80,50
scale = .3,.3
sprpriority = 8

[State 1005,ex ]
type = Explod
trigger1 = timemod = 1,0
anim = 7010 
pos = 0,0
vel = -.1*(Vel x),-.1*(vel Y)
random = 50,50
scale = .2,.2
sprpriority = 8

[State 1005,ds ]
type = DestroySelf
trigger1 = time = 12
trigger2 = parent,stateno !=  [1001,1011]

[Statedef 1006]
type = A
movetype = I
physics = N
velset = 5,0
anim = 8888
ctrl = 0

[State 1005,ex ]
type = Explod
trigger1 = timemod = 1,0
anim = 7010 
pos = 0,0
vel = 6,-8
random = 80,0
scale = .3,.3
sprpriority = 8

[State 1005,ex ]
type = Explod
trigger1 = timemod = 1,0
anim = 7010 
pos = 0,0
vel = 4,-5
random = 50,0
scale = .2,.2
sprpriority = 8

[State 1005,ex ]
type = Explod
trigger1 = timemod = 1,0
anim = 7010 
pos = 0,0
vel = 3,-3
random = 50,0
scale = .5,.5
sprpriority = 8

[State 1005,ds ]
type = DestroySelf
trigger1 = time = 12
trigger2 = parent,stateno != [1001,1011]

[Statedef 1010]
type = S
movetype = I 
physics = S
poweradd = 100*ifelse(var(0) > 0,2,1)
velset = 0,0
anim = 1000
ctrl = 0
juggle = 15

[State -2, Jumping Sound]
type = PlaySnd
trigger1 = animtime = 0
value = 40, 1

[State 1000,rd ]
type = ReversalDef
trigger1 = time = 0 
reversal.attr = SCA,NA,SA
numhits = 0
pausetime = 0,0

[State 1000,ex ]
type = Explod
trigger1 = animelem = 2
anim = 1002 
ID = 1002
bindtime = -1
sprpriority = 5
ownpal = 1
removeongethit = 1

[State 1001,st ]
type = StateTypeSet
trigger1 = animelem = 4
movetype = A

[State 1000,va ]
type = VelAdd
trigger1 = animelem = 2,>0
x = 3

[State 1000,st ]
type = ChangeState
trigger1 = animtime = 0
value = 1011

[Statedef 1011]
type = A
movetype = A 
physics = A
velset = 3,-8
anim = 1001
juggle = 15
ctrl = 0
sprpriority = 3

[State 1070,nh ]
type = NotHitBy
trigger1 = time >= 0
value = SCA,NA,SA
value2 = SCA, NT, ST, HT


[State 190, ]
type = PlaySnd
trigger1 = time = 0
value = 1000,0
channel = 0

[State 1001,re ]
type = RemoveExplod
trigger1 = time = 0
ID = 1002

[State 1001,help ]
type = Helper
trigger1 = time = 0 && pos Y > -5
name = "many explods 2"
ID = 1000
stateno = 1016
ownpal = 1
pos = 0,0

[State 1001,help ]
type = Helper
trigger1 = time = 0
name = "many explods"
ID = 1000
stateno = 1015
ownpal = 1
pos = 40,-54

[State 1001,sn ]
type = PlaySnd
trigger1 = time = 0
value = 1,5

[State 1001,ps ]
type = PosAdd
trigger1 = time = 0
x = 10

[State 1001,hd ]
type = HitDef
trigger1 = animelem = 1
attr = A,SA
hitflag = MAF
guardflag = MA
animtype = up
air.animtype = diagUp
damage = 160,7
pausetime = 3,5
sparkno = S70021
sparkxy = 0,-80
hitsound = S0,5
guardsound = 6,0
ground.velocity = -4,-8
air.velocity = -4,-8
fall = 1
fall.recover = 0
getpower = 30,0
givepower = 0,30

[State 1000,ex ]
type = Explod
trigger1 = movehit = 1
anim = F12
pos = ceil(p2dist X),-80
bindtime = 1
ontop = 1
sprpriority = 5
ownpal = 1
persistent = 0

[State 1000,ex ]
type = Explod
trigger1 = movehit = 1
anim = F11
pos = ceil(p2dist X),-70
bindtime = 1
ontop = 1
sprpriority = 5
ownpal = 1
persistent = 0

[State 1000,ex ]
type = Explod
trigger1 = time = 0
anim = 1003
bindtime = -1
removetime = 3
sprpriority = 5
ownpal = 1
removeongethit = 1

[State 1000,ex ]
type = Explod
trigger1 = animelem = 1
anim = 1004
bindtime = -1
scale = 1,1
sprpriority = 1
ownpal = 1
removeongethit = 1

[State 1000,ex ]
type = Explod
trigger1 = animelem = 2
anim = 1005
bindtime = -1
scale = 1,1
sprpriority = 1
ownpal = 1
removeongethit = 1

[State 1000,ex ]
type = null;Explod
trigger1 = animelem = 2
trigger2 = animelem = 3
anim = 1006
bindtime = 1
scale = 1,1.2
sprpriority = 1
ownpal = 1
removeongethit = 1

[Statedef 1015]
type = A
movetype = I
physics = N
velset = 12,0
anim = 8888
ctrl = 0

[State 1005,va ]
type = VelAdd
trigger1 = time > 0
x = -1
y = -1.9

[State 1005,ex ]
type = Explod
trigger1 = timemod = 1,0
anim = 7010 
pos = 0,0
vel = -.1*(Vel x),-.1*(vel Y)
random = 80,50
scale = .3,.3
sprpriority = 8

[State 1005,ex ]
type = Explod
trigger1 = timemod = 1,0
anim = 7010 
pos = 0,0
vel = -.1*(Vel x),-.1*(vel Y)
random = 50,50
scale = .2,.2
sprpriority = 8

[State 1005,ds ]
type = DestroySelf
trigger1 = time = 12
trigger2 = parent,stateno !=  [1001,1011]

[Statedef 1016]
type = A
movetype = I
physics = N
velset = 12,0
anim = 8888
ctrl = 0

[State 1005,ex ]
type = Explod
trigger1 = timemod = 1,0
anim = 7010 
pos = 0,0
vel = 6,-8
random = 80,0
scale = .5,.5
sprpriority = 8

[State 1005,ex ]
type = Explod
trigger1 = timemod = 1,0
anim = 7010 
pos = 0,0
vel = 4,-5
random = 50,0
scale = .2,.2
sprpriority = 8

[State 1005,ex ]
type = Explod
trigger1 = timemod = 1,0
anim = 7010 
pos = 0,0
vel = 3,-3
random = 50,0
scale = .6,.6
sprpriority = 8

[State 1005,ds ]
type = DestroySelf
trigger1 = time = 12
trigger2 = parent,stateno != [1001,1011]



;Spirit_Summon
[Statedef 1050]
type = S
movetype = I 
physics = S
velset = 0,0
anim = 1050
ctrl = 0
poweradd = 100*ifelse(var(0) > 0,2,1)


[State 11050,sn ]
type = PlaySnd
trigger1 =animelem = 2
value = 1100,4


[State 1050,ss ]
type = Helper
trigger1 = animelem = 2
helpertype = normal
name = "Spirit"
ID = 1050
pos = 20,-130
stateno = 11050
ownpal = 1

[State 1050,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1

;spirit burst
[Statedef 1060]
type = S
movetype = A
physics = S
velset = 0,0
anim = 1060
ctrl = 0
poweradd = 100*ifelse(var(0) > 0,2,1)

[State 1060,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 2,16
channel = 0

[State 1060,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 1060,0

[State 1050,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1

;soul lance
[Statedef 1070]
type = S
movetype = A
physics = N
poweradd = 100*ifelse(var(0) > 0,2,1)
velset = 0,0
anim = 1070
ctrl = 0

[State 1070,nh ]
type = NotHitBy
trigger1 = time >= 0
value = SCA,NA,SA,HA, NP, SP, HP
value2 = SCA, NT, ST, HT

[State 240,ht ]
type = HitDef
trigger1 = time = 0
attr = S,SA
hitflag = MA
guardflag = MA
animtype = Hard
priority = 6
damage = 30+(20*numhelper(1050)),8
pausetime = 10,12
sparkno = 2
sparkxy = 0,-78
hitsound = 5,3
guardsound = 6,0
ground.type = High
air.type = low
ground.slidetime = 17
ground.hittime = 17
ground.velocity = -5,0
air.velocity = -3.5,-6
ground.cornerpush.veloff = 0
air.cornerpush.veloff = 0
getpower = 30,5
givepower = 5,30
p1stateno = 1071

[State 1070,ex ]
type = Explod
trigger1 = (animelem = 3,>0) && (animelem = 5,<0)
anim = 7010
bindtime = 1
vel = -(3+(random%3)),0
pos = 51,-78
random = 0, 50
removetime = -2
scale = .5,.5
sprpriority = 7
ownpal = 1
removeongethit = 1

[State 1070,ex ]
type = Explod
trigger1 = animelem = 3
anim = 1071
ID = 1070
bindtime = -1
pos = 71,-78
removetime = -1
sprpriority = 7
ownpal = 1
scale = 1,.5
removeongethit = 1

[State 1070, ex]
type = RemoveExplod
trigger1 = animelem = 6
ID = 1070

[State 1070,m ]
type = VelMul
trigger1 = time > 16
x = .8

[State 1070, ]
type = VelSet
trigger1 = animelem = 3
x = 15

[State 1060,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 1060,0


[State 1050,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1

[Statedef 1071]
type = S
movetype = A
physics = S
velset = 8,0
anim = 1072
ctrl = 0
juggle = 0

[State 1060,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 2,12
channel = 0

[State 3002,ex ]
type = Explod
trigger1 = animelem = 7
anim = 3003
bindtime = 1
ID = 3000
pos = 20,0
postype = p1
sprpriority = -2
ownpal = 1
supermovetime = 30
removeongethit = 1

[State 240,ht ]
type = HitDef
trigger1 = time = 0
attr = S,SA
hitflag = MAF
guardflag = MA
animtype = Hard
priority = 6
damage = 100,8
pausetime = 10,12
sparkno = 2
sparkxy = 0,-78
hitsound = 5,3
guardsound = 6,0
ground.type = High
air.type = low
ground.slidetime = 17
ground.hittime = 17
ground.velocity = -2,-9
air.velocity = -3.5,-6
ground.cornerpush.veloff = -14
air.cornerpush.veloff = -6
getpower = 30,5
givepower = 5,30
fall = 1
fall.recovertime = 12

[State 1070, ex]
type = RemoveExplod
trigger1 = time = 0
ID = 1070

[State 1060,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 200,1

[State 1050,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1


;soul lance
[Statedef 1080]
type = S
movetype = A
physics = N
poweradd = 100*ifelse(var(0) > 0,2,1)
velset = 0,0
anim = 1070
ctrl = 0
juggle = 5

[State 1060,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 2,10

[State 1070,nh ]
type = NotHitBy
trigger1 = time >= 0
value = SCA,NA,SA,HA, NP, SP, HP
value2 = SCA, NT, ST, HT


[State 240,ht ]
type = HitDef
trigger1 = timemod = 2,0
attr = S,SA
hitflag = MAF
guardflag = MA
animtype = Hard
priority = 6
damage = 30+(5*numhelper(1050)),8
pausetime = 3,6
sparkno = 2
sparkxy = -30,-78
hitsound = 5,3
guardsound = 6,0
ground.type = High
air.type = low
ground.slidetime = 17
ground.hittime = 17
ground.velocity = -5,0
air.velocity = -3.5,-6
fall = 1
fall.recover = 0
ground.cornerpush.veloff = 0
air.cornerpush.veloff = 0
getpower = 30,5
givepower = 5,30

[State 1070,ex ]
type = Explod
trigger1 = animelem = 4
anim = 1071
ID = 1070
bindtime = -1
pos = 71,-78
removetime = -1
sprpriority = 7
ownpal = 1
removeongethit = 1
scale = 1,.5

[State 1070,ex ]
type = Explod
trigger1 = (animelem = 3,>0) && (animelem = 5,<0)
anim = 7010
bindtime = 1
vel = -(3+(random%3)),0
pos = 51,-78
random = 0, 50
removetime = -2
scale = .5,.5
sprpriority = 7
ownpal = 1
removeongethit = 1

[State 1070, ex]
type = RemoveExplod
trigger1 = animelem = 6
ID = 1070

[State 1070,m ]
type = VelMul
trigger1 = time > 16
x = .8

[State 1070, ]
type = VelSet
trigger1 = animelem = 3
x = 15

[State 1060,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 1060,0

[State 1050,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1


;SOUL ORB STATES
[Statedef 11050]
type = S
movetype = I 
physics = N
velset = 0,0
anim = 8888
ctrl = 0


[State 11050, ]
type = BindToParent
trigger1 = time > 0
pos = 20,-130

[State 11050,vs ]
type = VarSet
trigger1 = time = 0
v =  2
value = 24

[State 11050,vs ]
type = Varadd
trigger1 = (timemod = 1,0) && (time < 10)
v =  2
value = -2

[State 11050,vs ]
type = VarSet
trigger1 = time = 0
v =  1
value = -250

[State 11050,vs ]
type = Varadd
trigger1 = (timemod = 1,0) && (time < 10)
v =  1
value = 30

[State 11050,pf ]
type = PalFX
trigger1 = time > 0
time = 1
add = var(1),Var(1),var(1)

[State 1150,ex ]
type = Explod
trigger1 = time = 0
anim = 1051
ID = 1050
bindtime = -1
removetime = -1
scale = .1*(Var(2)),.1*(Var(2))
sprpriority = -2
ownpal = 0

[State 11050, ]
type = ModifyExplod
trigger1 = time > 0
id = 1050
scale = .1*(Var(2)),.1*(Var(2))

[State 11050,st ]
type = ChangeState
trigger1 = parent,stateno = [5000,5120]
value = 11051

[State 11050,st ]
type = ChangeState
trigger1 = (parent,stateno = 1050) && (parent,animelem = 3)
trigger2 = (parent, stateno != 1050) && (time = 10)
value = 11052

[State 11050,st ]
type = ChangeState
trigger1 = (parent,stateno = 3100) && (time = 10)
value = 13100

[Statedef 11051]
type = S
movetype = I 
physics = N
velset = 0,0
anim = 8888
ctrl = 0

[State 11050,vs ]
type = VarSet
trigger1 = time = 0
v =  2
value = 10

[State 11050,vs ]
type = Varadd
trigger1 = (timemod = 1,0) && (time < 10)
v =  2
value = -1

[State 11050,vs ]
type = VarSet
trigger1 = time = 0
v =  1
value = 0

[State 11050,vs ]
type = Varadd
trigger1 = (timemod = 1,0) && (time < 14)
v =  1
value = -15

[State 11050,pf ]
type = PalFX
trigger1 = time > 0
time = 1
add = var(1),Var(1),var(1)

[State 1150,ex ]
type = Explod
trigger1 = time = 0
anim = 1051
ID = 1050
bindtime = -1
removetime = -1
scale = .1*(Var(2)),.1*(Var(2))
sprpriority = 7
ownpal = 0
supermovetime = 30

[State 11050, ]
type = ModifyExplod
trigger1 = time > 0
id = 1050
scale = .1*(Var(2)),.1*(Var(2))

[State 11060,ex ]
type = RemoveExplod
trigger1 = time = 13
ID = 1050


[State 11050,st ]
type = destroyself
trigger1 = time = 14

[Statedef 11052]
type = S
movetype = I 
physics = N
velset = 2,2
anim = 8888
ctrl = 0

[State 11051,ex ]
type = Explod
trigger1 = (timemod = 2,0) && (numexplod < 6)
anim = 7010
vel = 0,-.4
random = 20,20
scale = 1, 1
sprpriority = -1
ownpal = 1


[State 11052,vs ]
type = VelSet
trigger1 = time = 10
x = 0
y = 0

[State 11052,va ]
type = VelAdd
trigger1 = (parentdist Y <= 70) && (time > 10) && (vel Y > -5)
y = -.5

[State 11052,va ]
type = VelAdd
trigger1 = (parentdist X >= -20) && (time > 10) && (vel X < 5)
X = .3

[State 11052,va ]
type = VelAdd
trigger1 = (parentdist X <= 20) && (time > 10) && (vel X > -5)
X = -.3

[State 11052,va ]
type = VelAdd
trigger1 = (parentdist Y >= 70) && (time > 10) && (vel Y < 5)
y = .5


[State 11052,t ]
type = Turn
trigger1 = facing != (parent,facing)

[State 11050,st ]
type = ChangeState
trigger1 = (parent,stateno = 1060) && (parent,animelem = 4)
value = 11060

[State 11050,st ]
type = ChangeState
trigger1 = (parent,stateno = 1070) && (parent,animelem = 3)
trigger2 = roundstate != 2
value = 11070

[State 11050,st ]
type = ChangeState
trigger1 = parent,stateno = 3000
value = 13000

[Statedef 11060]
type = S
movetype = A 
physics = N
velset = 0,0
anim = 8889
ctrl = 0
sprpriority = 8

[State 11060,hp ]
type = Helper
trigger1 = time = 0
name = "soul orb projectile"
ID = 1060
stateno = 11061
ownpal = 1

[State \11060,re ]
type = RemoveExplod
trigger1 = time = 0

[State 11060,ds ]
type = DestroySelf
trigger1 = time = 1

[Statedef 11061]
type = S
movetype = A 
physics = N
velset = 0,0
anim = 8889
ctrl = 0
sprpriority = 8

[State 11061,nhb ]
type = NotHitBy
trigger1 = time >= 0
value = SCA, AA,AT

[State 11051,ex ]
type = Explod
trigger1 = (timemod = 2,0) && (numexplod < 6)
anim = 7010
vel = 0,-.4
random = 20,20
scale = 1, 1
sprpriority = -1
ownpal = 1

[State 11051,ex ]
type = Explod
trigger1 = (timemod = 6,0) && (numexplod < 6)
anim = 7010
vel = 0,-.6
random = 20,20
scale = 1.7, 1.7
sprpriority = -1
ownpal = 1

[State 1150,ex ]
type = Explod
trigger1 = time = 0
anim = 1051
ID = 1050
bindtime = -1
removetime = -1
scale = .5,.5
sprpriority = 7
ownpal = 0
supermovetime = 30

[State 10060, ]
type = VelAdd
trigger1 = p2bodydist X  > 0
x = .2

[State 10060, ]
type = VelAdd
trigger1 = p2bodydist X < 0
x = -.2

[State 10060, ]
type = VelAdd
trigger1 = p2bodydist Y > 50
y = .2

[State 10060, ]
type = VelAdd
trigger1 = p2bodydist Y < 50
y = -.2

[State 11060,pj ]
type = Hitdef
trigger1 = time = 0
attr = S,SP
hitflag = MAFPD
guardflag = MA
animtype = hard
damage = 50,10
pausetime = 0,0
sparkno = 2
sparkxy = 0,0
hitsound = 5,4
guardsound = 6,0
ground.type = High
air.type = low
ground.slidetime = 20
ground.hittime = 20
ground.velocity = -2,-8
air.velocity = -2,-4
p1stateno = 11070
fall = 1
fall.recover = 1
fall.recovertime = 12
getpower = 30,10
givepower = 10,30
envshake.time = 10
envshake.freq = 60
envshake.ampl = -4
envshake.phase = 1

[State 11050,st ]
type = changestate
trigger1 = time = 180
trigger2 = movecontact
trigger3 = frontedgedist < -80
value = 11070

[Statedef 13100]
type = S
movetype = I 
physics = N
velset = 0,0
anim = 8888
ctrl = 0
sprpriority = -6

[State 13100,vr ]
type = posadd
trigger1 = time = 0
x = var(6)
y = var(6)

[State 13100,vr ]
type = VarRandom
trigger1 = time = 0
v = 6
range = -20,20

[State 13100,vr ]
type = VarRandom
trigger1 = time = 0
v = 7
range = -40,10

[State 13100,vr ]
type = VarRandom
trigger1 = time = 0
v = 9
range = -9,0

[State 13100,vr ]
type = VarRandom
trigger1 = time = 0
v = 10
range = -3,3

[State 11050,pf ]
type = PalFX
trigger1 = time > 0
time = 1
add = var(1),Var(1),var(1)

[State 11050,vs ]
type = VarSet
trigger1 = time = 0
v =  2
value = 0

[State 11050,vs ]
type = Varadd
trigger1 = (timemod = 1,0) && (time < 10)
v =  2
value = 1

[State 11050,vs ]
type = VarSet
trigger1 = time = 0
v =  1
value = -250

[State 11050,vs ]
type = Varadd
trigger1 = (timemod = 1,0) && (time < 10)
v =  1
value = 30

[State 1150,ex ]
type = Explod
trigger1 = time = 0
anim = 1051
ID = 1050
bindtime = -1
removetime = -1
scale = .1*(Var(2)),.1*(Var(2))
sprpriority = 2
ownpal = 0

[State 11050, ]
type = ModifyExplod
trigger1 = time > 0
id = 1050
scale = .05*(Var(2)),.05*(Var(2))

[State 11050,st ]
type = ChangeState
trigger1 = parent,stateno = [5000,5120]
value = 13102

[State 11050,st ]
type = ChangeState
trigger1 = (parent,stateno = 3100) && (time = 10)
value = 13101


[Statedef 13101]
type = S
movetype = A 
physics = N
velset = var(10),var(9)
anim = 8889
ctrl = 0
sprpriority = 8

[State 1301,vr ]
type = VarRandom
trigger1 = time = 0
v = 1
range = 1,2

[State 13101, ps ]
type = PlaySnd
trigger1 = time = 0
value = 1060,0
channel = var(1)

[State 1150,ex ]
type = Explod
trigger1 = timemod = 2,0
anim = 7010;1052
ID = 1050
bindtime = 1
removetime = -2
scale = 1.8,1.8
sprpriority = 2
ownpal = 0

[State 10060, ]
type = VelAdd
trigger1 = p2bodydist X > 0 && time > 4
x = .6

[State 10060, ]
type = VelAdd
trigger1 = p2bodydist X < 0 && time > 4
x = -.6

[State 10060, ]
type = VelAdd
trigger1 = p2bodydist Y > 50 && time > 4
y = .6

[State 10060, ]
type = VelAdd
trigger1 = p2bodydist Y < 50 && time > 4
y = -.6

[State 11060,pj ]
type = Hitdef
trigger1 = time = 0
attr = S,SA
hitflag = MAFPD
guardflag = MA
animtype = hard
damage = 20,1
pausetime = 0,7
sparkno = 2
getpower = 0,0
givepower = 0,0
sparkxy = 0,0
hitsound = 5,4
guardsound = 6,0
ground.type = High
air.type = low
ground.slidetime = 20
ground.hittime = 20
ground.velocity = -2,-8
air.velocity = -2,-4
fall = 1
fall.recover = 0
getpower = 30,10
givepower = 10,30
envshake.time = 10
envshake.freq = 60
envshake.ampl = -4
envshake.phase = 1

[State 11050,st ]
type = changestate
trigger1 = movecontact = 1
trigger2 = time = 300
value = 13102

[State 11050,st ]
type = changestate
trigger1 = pos Y >= 0
value = 13103


[Statedef 13102]
type = S
movetype = I 
physics = N
velset = 0,0
anim = 1054
ctrl = 0

[State 1303,asp ]
type = AssertSpecial
trigger1 = time >= 0
flag = noshadow


[State 11060,ex ]
type = RemoveExplod
trigger1 = time = 0
ID = 1050

[State 11050,st ]
type = destroyself
trigger1 = animtime = 0

[Statedef 13103]
type = S
movetype = A 
physics = N
velset = 0,0
anim = 1053
ctrl = 0

[State 11060,pj ]
type = Hitdef
trigger1 = time = 0
attr = S,SA
hitflag = MAFPD
guardflag = L
animtype = hard
damage = 40,1
pausetime = 0,7
sparkno = 2
getpower = 0,0
givepower = 0,0
sparkxy = 0,0
hitsound = 5,4
guardsound = 6,0
ground.type = High
air.type = low
ground.slidetime = 20
ground.hittime = 20
ground.velocity = -2,-8
air.velocity = -2,-4
fall = 1
fall.recover = 1
fall.recovertime = 12
getpower = 30,10
givepower = 10,30
envshake.time = 10
envshake.freq = 60
envshake.ampl = -4
envshake.phase = 1

[State 1303,asp ]
type = AssertSpecial
trigger1 = time >= 0
flag = noshadow


[State 11060,ex ]
type = RemoveExplod
trigger1 = time = 0
ID = 1050

[State 11050,st ]
type = destroyself
trigger1 = animtime = 0

[Statedef 11070]
type = S
movetype = I 
physics = N
velset = 0,0
anim = 1054
ctrl = 0

[State 1303,asp ]
type = AssertSpecial
trigger1 = time >= 0
flag = noshadow


[State 11060,ex ]
type = RemoveExplod
trigger1 = time = 0
ID = 1050

[State 11050,st ]
type = destroyself
trigger1 = animtime = 0


[Statedef 13000]
type = S
movetype = I 
physics = N
velset = 0,0
anim = 8888
ctrl = 0

[State 13000,bp ]
type = BindToParent
trigger1 = parent,animelem = 4
time = 30
pos = -3,-145

[State 13000,st ]
type = Changestate
trigger1 = parent,animelem = 4
value = 11051



[Statedef 1100]
type = S
movetype = I 
physics = N
velset = 8,0
anim = 1100
poweradd = 150*ifelse(var(0) > 0,2,1)
ctrl = 0

[State -2, Jumping Sound]
type = PlaySnd
trigger1 = time = 0
value = 1100, 1

[State 1100,ex ]
type = Explod
trigger1 = time = 0
anim = 1826
pos = 0,-60
ownpal = 1

[State 1100,ex ]
type = Explod
trigger1 = time = 0
anim = 11100
ownpal = 1

[State 1100,nh ]
type = NotHitBy
trigger1 = time > 0
value = SCA,AA
value2 = SCA,AP


[State 13000,st ]
type = Changestate
trigger1 = animtime = 0
value = 1101

[Statedef 1101]
type = S
movetype = A
physics = S
velset = 0,0
anim = 1101
ctrl = 0
juggle = 15

[State 1100,throw ]
type = HitDef
trigger1 = time = 0
attr = S,ST
hitflag = H-
guardflag = 
priority = 4,miss
sparkno = 2
sparkxy = 0,-60
hitsound = 5,3
p1stateno = 1102   
p2facing = 1
p2stateno = 1103
fall = 1
numhits = 0
getpower = 0,0
givepower = 0,0

[State 1100,ex ]
type = Explod
trigger1 = time = 0
anim = 11101
ownpal = 1

[State 13000,st ]
type = Changestate
trigger1 = animtime = 0
value = 0
ctrl = 1

[Statedef 1110]
type = S
movetype = I 
physics = N
velset = 16,0
anim = 1100
ctrl = 0
poweradd = 150*ifelse(var(0) > 0,2,1)

[State 1100,ex ]
type = Explod
trigger1 = time = 0
anim = 1826
pos = 0,-60
ownpal = 1

[State -2, Jumping Sound]
type = PlaySnd
trigger1 = time = 0
value = 1100, 1

[State 1100,ex ]
type = Explod
trigger1 = time = 0
anim = 11100
ownpal = 1

[State 1100,nh ]
type = NotHitBy
trigger1 = time > 0
value = SCA,AA
value2 = SCA,AP


[State 13000,st ]
type = Changestate
trigger1 = animtime = 0
value = 1101

[State 13000,st ]
type = Changestate
trigger1 = animtime = 0
value = 0
ctrl = 1

[Statedef 1102]
type = S
movetype = A
physics = S
velset = 0,0
anim = 1102
ctrl = 0
sprpriority = 3

[State 1102,sn ]
type = PlaySnd
trigger1 = animelem = 3
value = 1100,2

[State 1102,sn ]
type = PlaySnd
trigger1 = animelem = 3
value = 1100,0

[State 1102,tla ]
type = TargetLifeAdd
trigger1 = animelem = 3 
value = -200

[State 1102,EX ]
type = Explod
trigger1 = animelem = 3
trigger2 = animelem = 3,2
anim = 1103
sprpriority = 7
ownpal = 1

[State 1102,EX ]
type = Explod
trigger1 = animelem = 3,6
trigger2 = animelem = 3,15
trigger3 = animelem = 3,29
anim = F11
sprpriority = 7
ontop = 1
pos = 25,-125
ownpal = 1

[State 1102,EX ]
type = Explod
trigger1 = animelem = 3
trigger2 = animelem = 3,7
trigger3 = animelem = 3, 21
anim = F12
sprpriority = 7
facing = -1
ontop = 1
pos = 25,-125
ownpal = 1

[State 1102,sn ]
type = PlaySnd
trigger1 = animelem =3
value = 0,0

[State 1102, ]
type = TargetBind
trigger1 = animelem = 1
time = 6
pos = 24,-88

[State 1102, ]
type = TargetBind
trigger1 = animelem = 2
time = 70
pos = 25,-125


[State 13000,st ]
type = Changestate
trigger1 = animtime = 0
value = 0
ctrl = 1

[Statedef 1103]
type = S
movetype = H
physics = N
ctrl = 0
sprpriority = 4

[State 1103,sbn ]
type = ScreenBound
trigger1 = time > 0
value = 0

[State 1103,anim2 ]
type = ChangeAnim2
trigger1 = time = 0
trigger2 = anim != 1115
value = 1115

[State 13000,st ]
type = Changestate
trigger1 = time = 100
value = 1104

[Statedef 1104]
type = A
movetype = I
physics = N
velset = -4,-4
ctrl = 0

[State 1103,pa ]
type = PosAdd
trigger1 = time = 0
y = 64

[State 1103, ca]
type = ChangeAnim
trigger1 = (selfanimexist(5052) = 1) && (Time = 0)
value = 5052

[State 1103,ca2 ]
type = ChangeAnim2
trigger1 = (selfanimexist(5052) = 0) && (Time = 0)
value = 1116

[State 1103, ca]
type = ChangeAnim
trigger1 = (selfanimexist(5062) = 1) && (Time = 12)
value = 5062

[State 1103,ca2 ]
type = ChangeAnim2
trigger1 = (selfanimexist(5062) = 0) && (Time = 12)
value = 1117

[State 1103,ga ]
type = Gravity
trigger1 = time > 2

[State 1103,ss ]
type = SelfState
trigger1 = (pos Y >= 0) && (time > 12)
value = 5100






;Wandering Spirit
[Statedef 3000]
type = S
movetype = I 
physics = S
velset = 0,0
anim = 3000
ctrl = 0

[State 170, snd]
type = PlaySnd
trigger1 = time = 0
value = 3000,0

[State 3000,ex ]
type = Explod
trigger1 = animelem = 4
anim = 7101
ID = 3000
pos = 0,-132
supermove = 1
sprpriority = -2
ownpal = 1
removeongethit = 1


[State 3000, ev]
type = EnvColor
trigger1 = animelem = 4
value = 255,255,255
time = 3
under = 1

[State 3000, ev]
type = EnvColor
trigger1 = animelem = 4,3
value = 0,0,0
time = 27
under = 1

[State 3000,sp ]
type = SuperPause
trigger1 = animelem = 4
time = 30
anim = S7100
sound = S750,2
pos = 0,-132
movetime = 30
darken = 0
p2defmul = 1
poweradd = -1000

[State 3000,st ]
type = ChangeState
trigger1 = animtime = 0
value = 3001

[Statedef 3001]
type = S
movetype = A 
physics = N
velset = 10,0
anim = 3002
ctrl = 0
sprpriority = 2
juggle = 5

[State 1800,sn ]
type = PlaySnd
trigger1 = time = 0
value = 1100,3

[State 3001,nhb ]
type = NotHitBy
trigger1 = time >= 0
value = SCA,AA,AP

[State 3002,ex ]
type = Explod
trigger1 = time = 0
anim = 3003
facing = -1
bindtime = 1
ID = 3000
pos = 20,-60
postype = p1
sprpriority = -2
ownpal = 1
supermovetime = 30
removeongethit = 1

[State 3002,ex ]
type = Explod
trigger1 = time = 0
anim = 1071
facing = 1
bindtime = -1
ID = 3001
pos = 60,-60
postype = p1
scale = 1.5,1.5
sprpriority = 3
ownpal = 1
removeongethit = 1
removetime = -1

[State 3002,HD ]
type = HitDef
trigger1 = timemod = 2,0
attr = S,HA
hitflag = MAF
guardflag = MA
animtype = Hard
priority = 6
damage = 20,2
pausetime = 2,2
sparkno = 2
sparkxy = -30,-70
hitsound = 5,2
guardsound = 6,0
ground.type = Low
ground.slidetime = 20
ground.hittime = 20
air.hittime = 20
ground.velocity = -1,-3
guard.velocity = -8
air.velocity = -1,-5
airguard.velocity = -4,-3
ground.cornerpush.veloff = -2
air.cornerpush.veloff = -2
getpower = 0,0
givepower = 0,0
envshake.time = 5
envshake.freq = 60
envshake.ampl = -4
envshake.phase = 5
fall = 1
fall.recover = 0

[State 3001,st ]
type = ChangeState
trigger1 = time = 15
trigger2 = (p2bodydist X < 20) && (Time > 10)
value = 3002

[Statedef 3002]
type = A
movetype = A 
physics = N
anim = 3001
ctrl = 0
juggle = 5
sprpriority = 6

[State 3500,H ]
type = Helper
trigger1 = movehit = 1 && numhelper(3000) < 1 
name = "FX"
ID = 3000
pos = 0,-70
stateno = 3502
keyctrl = 0
ownpal = 1
ignorehitpause = 1

[State 170, snd]
type = PlaySnd
trigger1 = time = 0
value = 3000,1

[State 3002,ex ]
type = removeExplod
trigger1 = animelem = 2
ID = 3001

[State 3002,va ]
type = VelAdd
trigger1 = time > 0
x = -.1

[State 3002,HD ]
type = HitDef
trigger1 = animelem = 2
attr = S,HA
hitflag = MAF
guardflag = MA
animtype = Hard
priority = 6
damage = 100,10
pausetime = 5,5
sparkno = 3
sparkxy = 0,-70
hitsound = 5,4
guardsound = 6,0
ground.type = Low
ground.slidetime = 20
ground.hittime = 20
air.hittime = 20
ground.velocity = -1,-8
guard.velocity = -8
air.velocity = -1,-5
airguard.velocity = -4,-3
ground.cornerpush.veloff = -2
air.cornerpush.veloff = -2
getpower = 0,0
givepower = 0,0
envshake.time = 30
envshake.freq = 60
envshake.ampl = -4
envshake.phase = 5

[State 3002,HD ]
type = HitDef
trigger1 = animelem = 3
attr = S,HA
hitflag = MAF
guardflag = MA
animtype = Diagup
priority = 7
damage = 60*var(3),16
pausetime = 12,12
ID =3000
sparkno = 3
sparkxy = 0,-70
hitsound = 5,4
guardsound = 6,0
ground.type = Low
ground.slidetime = 20
ground.hittime = 20
air.hittime = 20
ground.velocity = -4,-7
guard.velocity = -8
air.velocity = -4,-7
airguard.velocity = -4,-3
ground.cornerpush.veloff = -2
air.cornerpush.veloff = -2
getpower = 0,0
givepower = 0,0
envshake.time = 20
envshake.freq = 60
envshake.ampl = -15
envshake.phase = 5
fall = 1
fall.recover = 0

[State 3001,nhb ]
type = NotHitBy
trigger1 = time >= 0
value = SCA,AA,AP


[State 3002,ex ]
type = Explod
trigger1 = animelem = 3
anim = 3003
ID = 3000
pos = 0,0
postype = p1
sprpriority = 5
ownpal = 1
removeongethit = 1
ignorehitpause = 1

[State 3002,ex ]
type = Explod
trigger1 = animelem = 3
anim = 3003
ID = 3000
scale = 1.4,1.4
pos = 0,0
postype = p1
sprpriority = 5
ownpal = 1
removeongethit = 1
ignorehitpause = 1


[State 3002,va ]
type = VelAdd
trigger1 = time > 10
y = .5

[State 3002,va ]
type = Velset
trigger1 = animelem = 3
y = -10

[State 3002,sn ]
type = PlaySnd
trigger1 = animelem = 2
value = 200,1

[State 3002,ln ]
type = ChangeState
trigger1 = (pos Y >= 0) && (time > 10)
value = 52
ctrl = 1



;Point of No Return
[Statedef 3100]
type = S
movetype = I 
physics = S
velset = 0,0
anim = 3100
ctrl = 0
sprpriority = 3

[State 3100,nhb ]
type = NotHitBy
trigger1 = time >= 0
value = SCA,AA,AP


[State 750,ex ]
type = Explod
trigger1 = (animelem = 4,>= 30) && timemod = 5,0
anim = 753
ID = 750
bindtime = -1
sprpriority = 4
ownpal = 1
removeongethit = 1


[State 3100,sn ]
type = PlaySnd
trigger1 = time = 0
value = 3100,0
channel = 0

[State 3100,sn ]
type = PlaySnd
trigger1 = time = 40
value = 3100,1
channel = 0

[State 1050,ss ]
type = Helper
trigger1 = (time = [6,120]) && (Timemod = 8,0)
helpertype = normal
name = "Spirit"
ID = 3100
pos = 0,-132
stateno = 13100
ownpal = 1

[State 3000,ex ]
type = Explod
trigger1 = animelem = 4
anim = 7101
ID = 3000
pos = 0,-132
supermove = 1
sprpriority = -2
ownpal = 1
removeongethit = 1

[State 3000, ev]
type = EnvColor
trigger1 = animelem = 4
value = 255,255,255
time = 3
under = 1

[State 3000, ev]
type = EnvColor
trigger1 = animelem = 4,3
value = 0,0,0
time = 27
under = 1

[State 3000,sp ]
type = SuperPause
trigger1 = animelem = 4
time = 30
anim = S7100
sound = S750,2
pos = 0,-132
movetime = 30
darken = 0
p2defmul = 1
poweradd = -1000

[State 3100,st ]
type = ChangeState
trigger1 = animtime = 0
value = 0
ctrl = 1









[Statedef 3500]
type = S
movetype = A 
physics = S
velset = 0,0
anim = 3500
ctrl = 0
sprpriority = 2

[State 3000,ex ]
type = Explod
trigger1 = time = 60
anim = 1826
ID = 3000
pos = -15,-53
ownpal = 1
bindtime = 1
vel = -5,-5
accel = .3,.3
removetime = -2
facing = -1
sprpriority = -1

[State 3000,ex ]
type = Explod
trigger1 = time = 50
anim = 3506
ID = 3000
pos = -21,-93
ownpal = 1
bindtime = -1
removetime = 250
facing = -1
sprpriority = -1

[State 3000,ex ]
type = Explod
trigger1 = time = 50
anim = 3506
ID = 3000
pos = -11,-93
ownpal = 1
bindtime = -1
removetime = 250
facing = -1
sprpriority = -1
scale = .5,1

[State 3500,pa ]
type = PosAdd
trigger1 = time= 0 
x = -20

[State 170, snd]
type = PlaySnd
trigger1 = time = 0
value = 3500,1

[State 3000,ex ]
type = Explod
trigger1 = time = 0
anim = 7101
ID = 3000
pos = -20,-82
supermove = 1
sprpriority = -2
ownpal = 1
removeongethit = 1
bindtime = -1

[State 3000,ex ]
type = Explod
trigger1 = animelem = 3
anim = 3503
ID = 3000
pos = -13,-91
supermove = 1
sprpriority = -2
ownpal = 1
removeongethit = 1
bindtime = -1
removetime = 250

[State 3000,ex ]
type = Explod
trigger1 = time = 0
anim = 7201
postype = left
ID = 3000
pos = 0,0
supermove = 1
sprpriority = -1
ownpal = 1
removeongethit = 1
removetime = 50
bindtime = -1

[State 3000,ex ]
type = Explod
trigger1 = animelem = 3,1
anim = 3501
ID = 3000
pos = 0,0
supermove = 1
sprpriority = 4
ownpal = 1
removeongethit = 1

[State 3000, ev]
type = EnvColor
trigger1 = time = 0
value = 255,255,255
time = 3
under = 1

[State 3500,vr ]
type = VarRandom
trigger1 = timemod = 1,0
v = 1
range = 250,-250

[State 3500,vr ]
type = VarRandom
trigger1 = timemod = 1,0
v = 2
range = 30,-30

[State 3500,asp ]
type = AssertSpecial
trigger1 = 1
flag = noFG

[State 3500,H ]
type = Helper
trigger1 = animelem = 3,1
name = "Cross"
ID = 3500
pos = var(1),var(2)
stateno = 3501
keyctrl = 0
ownpal = 1
facing = ifelse(var(1)>0,1,-1)
size.xscale = ifelse(var(2)>0,1,.5)
size.yscale = ifelse(var(2)>0,1,.5)

[State 3000, ev]
type = EnvColor
trigger1 = time = [50,300]
value = 0,0,0
time = 1
under = 1

[State 3500,H ]
type = Helper
trigger1 = animelem = 3,1
trigger1 = time = [50,290]
name = "FX"
ID = 3500
pos = 0,-62
stateno = 3502
keyctrl = 0
ownpal = 1

[State 3000, ev]
type = EnvColor
trigger1 = time = 3
value = 0,0,0
time = 47
under = 1

[State 3500,hp ]
type = Helper
trigger1 = time = 0
name = "huge ass symbol"
pos = 0,-82
stateno = 35000
ownpal = 1
supermovetime = 30
facing = 1


[State 3000,sp ]
type = SuperPause
trigger1 = time = 0
time = 50
anim = S7100
sound = S3500,0
pos = -20,-82
movetime = 50
darken = 0
p2defmul = 1
poweradd = -2000

[State 3000,st ]
type = ChangeAnim
trigger1 = time = 300
value = 35001

[State 3500,pa ]
type = PosAdd
trigger1 = anim = 35001 && animtime = 0
x = 20

[State 3000,st ]
type = ChangeState
trigger1 = anim = 35001 && animtime = 0
value = 0
ctrl = 1

[Statedef 3502]
type = S
movetype = A 
physics = S
velset = 0,0
anim = 8888
ctrl = 0

[State 3000,ex ]
type = Explod
trigger1 = time = 0
anim = 3504
ID = 3000
pos = 0,0
ownpal = 1
bindtime = -1
removetime = 50
scale = .1*time,.1*time

[State 3000,ex ]
type = ModifyExplod
trigger1 = 1
ID = 3000 
scale = .09*time,.09*time

[State 3501,ds ]
type = DestroySelf
trigger1 = time = 50

[Statedef 3501]
type = S
movetype = A 
physics = S
velset = 0,0
anim = 35002+random%4
ctrl = 0
sprpriority = ifelse(pos Y > (parent, pos Y),4,-3)

[State 3501,SN ]
type = PlaySnd
trigger1 = time = 0
value = 3500,2
channel = 1


[State 1103,hb ]
type = HitBy
trigger1 = 1
value = SCA, AP
time = 1 

[State 1103,hb ]
type = HitOverride
trigger1 = 1
attr = SCA, NP, SP, HP
slot = 1

[State 240,ht ]
type = HitDef
trigger1 = time = 10
attr = S,HP
hitflag = MAFPD
guardflag = MA
animtype = Hard
damage = 30,7
pausetime = 0,2
sparkno = 2
sparkxy = 0,ceil(p2dist Y)-50
hitsound = 5,4
guardsound = 6,0
ground.type = high
air.type = low
ground.slidetime = 9
ground.hittime = 9
ground.velocity = 7*ifelse(Pos X > (parent, pos Y),1,-1),-7
air.velocity = 3*ifelse(Pos X > (parent, pos Y),1,-1),-3
fall = 1
fall.recover = 0
ground.cornerpush.veloff = 0
air.cornerpush.veloff = 0
getpower = 0,0
givepower = 5,30


[State 3501,pfx ]
type = PalFX
trigger1 = time = 20
time = 60
sinadd = -255,-255,-255,160

[State 3501,asp ]
type = AssertSpecial
trigger1 = 1
flag = noshadow
flag2 = invisible

[State 3000,ex ]
type = Explod
trigger1 = time = 0
anim = anim
ID = 3000
pos = 0,0
ownpal = 0
bindtime = -1
removetime = 60
scale = .1*var(1),.1*var(1)
sprpriority = ifelse(pos Y > (parent, pos Y),4,-3)

[State 3000,ex ]
type = Explod
trigger1 = time = 0
anim = 1826
pos = 0,0
ownpal = 1
bindtime = 1
removetime = -2
vel = 2,-4
sprpriority = 5
accel = 0,.1

[State 3000,ex ]
type = Explod
trigger1 = time = 0
anim = 1054
pos = 0,0
ownpal = 1
bindtime = -1
removetime = -2
sprpriority = 5

[State 3000,ex ]
type = ModifyExplod
trigger1 = 1
ID = 3000 
scale = .1*var(1),.1*var(1)

[State 3000,ex ]
type = VarAdd
trigger1 = time < (((const(size.xscale))*10)/2)
v =  1
value = 2


[State 3501,ds ]
type = DestroySelf
trigger1 = time = 60


[Statedef 35000]
type = S
velset = 0,0
anim = 8888
ctrl = 0
sprpriority = 7

[State 35000,pf ]
type = PalFX
trigger1 = time > 0
time = 1
add = var(2),var(2),var(2)
mul = 256,256,256

[State 50000,vs ]
type = VarSet
trigger1 = time = 0
v =  2
value = -255

[State 500000,va ]
type = VarAdd
trigger1 = (time > 0) && (var(2) < 0)
v =  2
value = 12

[State 35000,ex ]
type = Explod
trigger1 = time = 0 
trigger2 = time = 1 
anim = 3602
ID = 35000
pos = 0,0
removetime = 30
scale = .1*(var(1)),.1*(var(1))
sprpriority = 7
removeongethit = 1
ownpal = 0

[State 35000,ex ]
type = ModifyExplod
trigger1 = time > 0
ID = 35000
scale = .1*(var(1)),.1*(var(1))
removetime = 30

[State 35000,1 ]
type = Varset
trigger1 = time = 0
v =  1
value = 300

[State 35000, 2]
type = VarAdd
trigger1 = timemod = 1,0
v =  1
value = -10

[State 35000,ds ]
type = DestroySelf
trigger1 = time = 30

;---------------------------------------------------------------------------
; Override common states (use same number to override) :
;---------------------------------------------------------------------------

[Statedef 105]
type    = A
physics = A
ctrl = 0
anim = 105
sprpriority = 1

[State 105, 1]
type = VelSet
trigger1 = Time = 0
x = const(velocity.run.back.x)
y = const(velocity.run.back.y)

[State 105, 2]
type = PlaySnd
trigger1 = Time = 0
value = 40,1

[State 105, 3]
type = ChangeState
trigger1 = Vel Y > 0
trigger1 = Pos Y >= 0
value = 106

[State 105,4 ]
type = VelMul
trigger1 = time > 0
x = .95

[State 105,4 ]
type = VelAdd
trigger1 = time > 0
y = .1

;---------------------------------------------------------------------------
;RUN BACK OVERRIDE
[Statedef 105]
type    = A
physics = A
ctrl = 0
anim = 105
sprpriority = 1

[State 105, 1]
type = VelSet
trigger1 = Time = 0
x = const(velocity.run.back.x)
y = const(velocity.run.back.y)



[State 105, 3]
type = ChangeState
trigger1 = Vel Y > 0
trigger1 = Pos Y >= 0
value = 106

[State 105,4 ]
type = VelMul
trigger1 = time > 0
x = .87

[State 105,4 ]
type = VelAdd
trigger1 = time > 0
y = .1

; RUN_BACK2 (land)
[Statedef 106]
type    = S
physics = S
ctrl = 0
anim = 0

[State 106, 1]
type = VelSet
trigger1 = Time = 0
y = 0

[State 106, 2]
type = PosSet
trigger1 = Time = 0
y = 0

[State 106, 3]
type = MakeDust
trigger1 = Time = 0
pos = -5,-2
spacing = 1

[State 106, 4]
type = ChangeState
trigger1 = Time = 1
value = 0
ctrl = 1




;AI ACTIVATION CODES


[StateDef 9741]
type = S
movetype = I
physics = N
anim = 9741
ctrl = 0

[State 9741, Safety]; Just in case your opponent is very poorly coded.
type = SelfState
trigger1 = !IsHelper
value = 5050

[State 9741, UglyHack]
type = VarSet
trigger1 = !var(1)
var(2) = 2

[State 9741, PauseCheck]
type = VarSet
trigger1 = GameTime > Parent,var(57)
trigger2 = !(Parent,Alive)
trigger3 = RoundState != 2
trigger4 = Parent,var(58)
trigger5 = var(3):=0
var(2) = (var(3):=1)+(var(4):=0)

[State 9741, Facing]
type = Turn
trigger1 = var(9):=(Facing != Parent,Facing)
trigger1 = !(var(4):=0)

[State 9741, DirTurnOnAI]
type = ParentVarSet
triggerall = !(var(3) || var(9))
trigger1 = Parent,command = "holdfwd"
trigger2 = Parent,command = "holdback"
trigger3 = Parent,command = "holdup"
trigger4 = Parent,command = "holddown"
trigger5 = !(var(4):=1)
var(58) = var(4)

[State 9741, DirTurnOffAI]
type = ParentVarSet
triggerall = !var(3)
triggerall = Parent,var(58)
trigger1 = command = "holdfwd"
trigger1 = Parent,command = "holdfwd"
trigger2 = command = "holdback"
trigger2 = Parent,command = "holdback"
trigger3 = command = "holdup"
trigger3 = Parent,command = "holdup"
trigger4 = command = "holddown"
trigger4 = Parent,command = "holddown"
var(58) = var(4):=0

[State 9741, ButtonTurnOnAI]
type = ParentVarSet
triggerall = !var(2)
trigger1 = Parent,command = "a"
trigger2 = Parent,command = "b"
trigger3 = Parent,command = "c"
trigger4 = Parent,command = "x"
trigger5 = Parent,command = "y"
trigger6 = Parent,command = "z"
trigger7 = Parent,command = "start"
var(58) = 1

[State 9741, ButtonTurnOffAI]
type = ParentVarSet
triggerall = !var(2)
trigger1 = command = "a"
trigger1 = Parent,command = "a"
trigger2 = command = "b"
trigger2 = Parent,command = "b"
trigger3 = command = "c"
trigger3 = Parent,command = "c"
trigger4 = command = "x"
trigger4 = Parent,command = "x"
trigger5 = command = "y"
trigger5 = Parent,command = "y"
trigger6 = command = "z"
trigger6 = Parent,command = "z"
trigger7 = command = "start"
trigger7 = Parent,command = "start"
var(58) = -1

[State 9741, UnPauseCheck]
type = VarSet
triggerall = var(2)
trigger1 = Parent,command = "a"
trigger2 = Parent,command = "b"
trigger3 = Parent,command = "c"
trigger4 = Parent,command = "x"
trigger5 = Parent,command = "y"
trigger6 = Parent,command = "z"
trigger7 = Parent,command = "start"
trigger8 = var(2) = 2
trigger8 = var(2):=0
var(2) = 2

[State 9741, Goodbye]
type = DestroySelf
triggerall = var(1):=1
trigger1 = Parent,var(58)
trigger2 = !(Parent,Alive)
trigger3 = RoundState != 2

[StateDef 9742]
type = S
movetype = I
physics = N
anim = 9741
ctrl = 0

[State 9742, Safety]; Just in case your opponent is very poorly coded.
type = SelfState
trigger1 = !IsHelper
value = 5050

[State 9742, UglyHack]
type = VarSet
trigger1 = !var(1)
var(2) = var(3):=2

[State 9742, PauseCheck]
type = VarSet
trigger1 = GameTime > Parent,var(57)
trigger2 = !(Parent,Alive)
trigger3 = RoundState != 2
trigger4 = Parent,var(58)
trigger5 = var(3):=0
var(2) = var(3):=1

[State 9742, Facing]
type = Turn
trigger1 = var(9):=(Facing != Parent,Facing)

[State 9742, ParentDirCheck]
type = VarSet
trigger1 = Parent,command = "holdfwd"
trigger2 = Parent,command = "holdback"
trigger3 = Parent,command = "holdup"
trigger4 = Parent,command = "holddown"
trigger5 = var(6):=0
var(6) = 1

[State 9742, BufferCheck]
type = VarSet
trigger1 = var(3) || var(4) || var(10)
var(10) = var(6)

[State 9742, ParentButtonCheck]
type = VarSet
trigger1 = Parent,command = "a"
trigger2 = Parent,command = "b"
trigger3 = Parent,command = "c"
trigger4 = Parent,command = "x"
trigger5 = Parent,command = "y"
trigger6 = Parent,command = "z"
trigger7 = Parent,command = "start"
trigger8 = var(7):=0
var(7) = 1

[State 9742, SameButtonCheck]
type = VarSet
triggerall = var(7)
triggerall = !var(2)
triggerall = NumPartner
trigger1 = Parent,command = "a"
trigger1 = Partner,command = "a"
trigger2 = Parent,command = "b"
trigger2 = Partner,command = "b"
trigger3 = Parent,command = "c"
trigger3 = Partner,command = "c"
trigger4 = Parent,command = "x"
trigger4 = Partner,command = "x"
trigger5 = Parent,command = "y"
trigger5 = Partner,command = "y"
trigger6 = Parent,command = "z"
trigger6 = Partner,command = "z"
trigger7 = Parent,command = "start"
trigger7 = Partner,command = "start"
trigger8 = var(8):=0
var(8) = 1

[State 9742, DirTurnOnAI]
type = ParentVarSet
trigger1 = var(6)
trigger1 = !(var(9) || var(10))
var(58) = 1

[State 9742, DirTurnOffAI]
type = ParentVarSet
triggerall = var(6)
triggerall = !var(3)
triggerall = Parent,var(58)
trigger1 = command = "holdfwd"
trigger1 = Parent,command = "holdfwd"
trigger2 = command = "holdback"
trigger2 = Parent,command = "holdback"
trigger3 = command = "holdup"
trigger3 = Parent,command = "holdup"
trigger4 = command = "holddown"
trigger4 = Parent,command = "holddown"
var(58) = 0

[State 9742, ButtonTurnOnAI]
type = ParentVarSet
trigger1 = var(5) && var(7)
trigger1 = !(var(2) || var(8))
var(58) = 1

[State 9742, ButtonTurnOffAI]
type = ParentVarSet
triggerall = var(7)
triggerall = !(var(2) || var(8))
trigger1 = command = "a"
trigger1 = Parent,command = "a"
trigger2 = command = "b"
trigger2 = Parent,command = "b"
trigger3 = command = "c"
trigger3 = Parent,command = "c"
trigger4 = command = "x"
trigger4 = Parent,command = "x"
trigger5 = command = "y"
trigger5 = Parent,command = "y"
trigger6 = command = "z"
trigger6 = Parent,command = "z"
trigger7 = command = "start"
trigger7 = Parent,command = "start"
var(58) = -1

[State 9742, UnPauseCheck]
type = VarSet
triggerall = var(2)
triggerall = !var(4)
trigger1 = var(7)
trigger2 = var(2) = 2
trigger2 = var(2):=0
var(2) = 2

[State 9742, Goodbye]
type = DestroySelf
trigger1 = Parent,var(58)
trigger2 = !(Parent,Alive)
trigger3 = RoundState != 2

[State 9742, HitPauseCheck]
type = VarSet
trigger1 = var(6) || var(7)
trigger2 = !(Parent,HitPauseTime)
var(4) = Parent,HitPauseTime

[State 9742, Delay]
type = VarSet
trigger1 = var(1):=1
var(5) = !var(7)




;LOW JUMP
[Statedef 900]
type    = A
physics = A
anim = 900

[State 900,v ]
type = Velmul
trigger1 = time = 0
y = .7

[State 50, 1]
type = VarSet
trigger1 = Time = 0
var(3) = 0

; Jump Start
[Statedef 40]
type    = S
physics = S
anim = 40
ctrl = 0
sprpriority = 1
facep2 = 1

[State 40, 1]
type = VarSet
trigger1 = Time = 0
var(3) = 0

[State 40, 2]
type = VarSet
trigger1 = command = "holdfwd"
trigger2 = (var(58) = 1) && (p2bodydist X > 70)
var(3) = 1

[State 40, 3]
type = VarSet
trigger1 = command = "holdback"
var(3) = -1

[State 40, 4]
type = VelSet
trigger1 = AnimTime = 0
x = ifelse(var(3)=0, const(velocity.jump.neu.x), ifelse(var(3)=1, const(velocity.jump.fwd.x), const(velocity.jump.back.x)))
y = const(velocity.jump.y)

[State 40, 5]
type = VelSet
trigger1 = AnimTime = 0
trigger1 = prevstateno = 100 ;RUN_FWD
trigger1 = var(3) = 1
x = const(velocity.runjump.fwd.x)

[State 40, 6]
type = ChangeState
trigger1 = (AnimTime = 0) && (command != "holdup")
value = 900
ctrl = 1

[State 40, 6]
type = ChangeState
trigger1 = AnimTime = 0
value = 50
ctrl = 1



;---------------------------------------------------------------------------
; States that are always executed (use statedef -2)
;---------------------------------------------------------------------------

[Statedef -2]


[State -2, Landing Sound]
type = PlaySnd
triggerall = Time = 1
trigger1 = stateno = 52 
value = 40, 0

[State -2, Jumping Sound]
type = PlaySnd
triggerall = Time = 1
trigger1 = stateno = 40 
value = 40, 1

[State 1801,asp ]
type = AssertSpecial
trigger1 = roundstate = 1
trigger2 = roundstate = 3
trigger3 = roundstate = 4
trigger4 = roundstate = 0
flag = nobardisplay
ignorehitpause = 1

[State -2,winquotes]
type = Helper
triggerall = Win && numhelper(1800) = 0 && time = 0 && (stateno = 180) && matchover
trigger1 = numpartner
trigger1 = ((partner, alive) ^^ (ID > partner, ID)) || (partner, life = 0)
trigger2 = !numpartner
helpertype = normal
name = "Winquotes"
ID = 180
pos = 0,0
postype = p1
facing = 1
stateno = 1800
ownpal = 1
ignorehitpause = 1
persistent = 0

;-AI CODES-

[State -2, StopAI]
type = VarSet
triggerall = var(58) > 0 && !IsHelper
trigger1 = RoundState != 2
trigger2 = !Alive
var(58) = -2
IgnoreHitPause = 1

[State -2, Simplifier]
type = VarSet
triggerall = !IsHelper
trigger1 = var(58) = 1
trigger2 = var(0):=0
var(0) = 1
IgnoreHitPause = 1

[State -2,EVIL]
type = Changestate
triggerall = var(0) 
triggerall = movetype != H
triggerall = ctrl && statetype != A
triggerall = p2bodydist X < 200
trigger1 = (hitdefattr = SC,NA) && power >= 2000 && (life < (const(data.life)/3))
trigger2 = power >= 2000 && (gametime%20 = 1) && (life < (const(data.life)/3))
value = 3500

[State -2,breakground]
type = Changestate
triggerall = var(0) 
triggerall = movetype != H
triggerall = enemy, movetype != H
triggerall = enemy,stateno != 5210
triggerall = enemy,prevstateno != [5030,5050]
triggerall = prevstateno != 47
trigger1 = (hitdefattr = SC,NA) && (movecontact) && (Stateno != 250)  && (p2bodydist X = [30,90]) && random < 100 && statetype != A && (gametime%20 = 1)
trigger2 = (hitdefattr = SC,NA) && (movecontact) && (Stateno != 250)  && p2bodydist X < 70 && (prevstateno = [120,140]) && statetype != A && frontedgedist > 30
value = 1010

[State -2, breakair]
type = Changestate
triggerall = var(0) 
triggerall = movetype != H
triggerall = enemy, movetype != H
triggerall = enemy,stateno != 5210
triggerall = enemy,prevstateno != [5030,5050]
triggerall = prevstateno != 47
trigger1 = p2bodydist X < 90 && p2statetype = A && ctrl = 1 && p2movetype != H && random < 700
value = 1000+1*statetype = A

[State -2,dodge]
type = Changestate
triggerall = var(0) = 1
trigger1 = statetype != A && p2movetype = A && ctrl && p2bodydist X < 70 && random < 50 && (prevstateno != [700,701])
trigger2 = statetype != A && p2statetype = C && ctrl && p2movetype = A && p2bodydist X < 70
value = 700+(random < 400)

[State -2, guard]
type = ChangeState
value = 132
triggerall = p2movetype = A || (enemy, NumProj >= 1)
triggerall = P2bodydist X <= 90 || (enemy, NumProj >= 1)
triggerall = statetype = A
triggerall = var(0)
triggerall = anim != 132
trigger1 = ctrl

[State -2, guard]
type = ChangeState
value = 131
;triggerall = p2movetype = A || (enemy, NumProj >= 1)
triggerall = enemy,hitdefattr = C, NA,SA
triggerall = P2bodydist X <= 90
triggerall = statetype != A
triggerall = var(0)
triggerall = ctrl
trigger1 = anim != 131
;trigger1 = enemy,statetype = C


[State -2,guard cancel]
type = Changestate
triggerall = var(0) = 1
trigger1 = (stateno = [150,151]) && (power > 1000) && (random < 800) && (p2bodydist X < 60) && (p2statetype != C) && life < 300
trigger2 = (stateno = [150,151]) && (power > 1000) && (p2bodydist X < 60) && gametime%60 = 1
value = 250


[State -2, guard]
type = ChangeState
value = 130
triggerall = p2movetype = A || (enemy, NumProj >= 1)
triggerall = P2bodydist X <= 90 || (enemy, NumProj >= 1)
triggerall = statetype != A
triggerall = var(0)
triggerall = ctrl
triggerall = anim != 130
trigger1 = p2statetype != C
trigger2 = (enemy, NumProj >= 1)

[State -2,hp-hk]
type = Changestate
triggerall = var(0) 
triggerall = movetype != H
trigger1 = (ctrl = 1) && (p2bodydist X < 20) && (p2movetype = I) && (p2statetype != L) && (Statetype != A) && p2stateno < 105 && p2statetype != C
value = 210+(((30+5*(p2bodydist X < 5 && statetype = S)))*(random > 200))

[State -2,hp-hk]
type = Changestate
triggerall = var(0) 
triggerall = movetype != H
trigger1 = (ctrl = 1) && (p2bodydist X < 20) && (p2movetype = I) && (p2statetype != L) && (Statetype != A) && p2stateno < 105 && p2statetype = C
value = 410

[State -2,hp-hk]
type = Changestate
triggerall = var(0) 
triggerall = movetype != H
triggerall = stateno != 220
trigger1 = (ctrl = 1) && (p2bodydist X < 50) &&  hitdefattr = SC,NA && movecontact
value = 220

[State -2,hp-hk]
type = Changestate
triggerall = var(0) 
triggerall = movetype != H
trigger1 = (ctrl = 1) && (p2bodydist X = [60,120]) && (p2movetype = A) && (p2statetype != L) && (Statetype != A)
value = 250

[State -2,hp-hk]
type = Changestate
triggerall = var(0) 
triggerall = movetype != H
trigger1 = (ctrl = 1) && (p2bodydist X = [20,50]) && (p2movetype = H) && (p2statetype = A) && (Statetype != A)
value = 410

[State -2,EVIL]
type = Changestate
triggerall = var(0) 
triggerall = movetype != H
triggerall = ctrl && statetype != A
trigger1 = p2bodydist X > 30 && (gametime%50 = 1) && numhelper(1050) > 1
value = 1060

[State -2,EVIL]
type = Changestate
triggerall = var(0) 
triggerall = movetype != H
triggerall = ctrl && statetype != A
trigger1 = p2bodydist X > 120 && p2movetype = A && (gametime%30 = 1)
trigger2 = prevstateno = 1060 && numhelper(1050) = 0 && (p2stateno = [120,140]) && p2statetype = S
trigger3 =  p2movetype = I && (p2stateno > 106) && (gametime%30 = 1)
value = 1110

[State -2,EVIL]
type = Changestate
triggerall = var(0) 
triggerall = movetype != H
trigger1= ctrl && statetype != A
trigger1 = p2bodydist X < 40 && p2movetype != A && (gametime%30 = 1)
trigger2 = hitdefattr = SC,NA && movecontact && random < 500 && stateno != 220 && prevstateno != 220
value = 220

[State -2,EVIL]
type = Changestate
triggerall = var(0) 
triggerall = movetype != H
trigger1= ctrl && statetype != A
trigger1 = p2bodydist X < 120 && p2movetype = A && (gametime%30 = 1)
trigger2 = stateno = 220 && hitcount = 2 && movecontact && random < 500
value = 1070+10*(random > 500)

[State -2,EVIL]
type = Changestate
triggerall = var(0) 
triggerall = movetype != H
triggerall = ctrl && statetype != A
trigger1 = p2bodydist X > 60 && p2movetype = A && (gametime%30 = 1)
value = 1100

[State -2,EVIL]
type = Changestate
triggerall = var(0) 
triggerall = movetype != H
triggerall = ctrl && statetype != A
trigger1 = p2bodydist X > 100 && power >= 1000 && p2movetype != H && (gametime%50 = 1)
value = 3100

[State -2,EVIL]
type = Changestate
triggerall = var(0) 
triggerall = movetype != H
triggerall = ctrl && statetype != A
trigger1 = (hitdefattr = SC,NA) && numhelper(1050) > 0 && power >= 1000 && (p2bodydist X < 100) && movecontact
value = 3000



;-END OF AI CODES-




[State -2,ex ]
type = Explod
trigger1 = anim = 20 && numexplod(20) = 0
anim = 7020
ID = 20
pos = 6,0
bindtime = -1
sprpriority = 3
ownpal = 1
removeongethit = 1
removetime = -1

[State -2,re ]
type = RemoveExplod
trigger1 = anim != 20
ID = 20

[State -2,ex ]
type = Explod
trigger1 = anim = 21 && numexplod(21) = 0
anim = 7020
ID = 21
facing = -1
pos = -16,0
bindtime = -1
sprpriority = 3
ownpal = 1
removeongethit = 1
removetime = -1

[State -2,re ]
type = RemoveExplod
trigger1 = anim != 21
ID = 21

[State -2,ex ]
type = Explod
trigger1 = anim = 100 && numexplod(100) = 0
anim = 7021
ID = 100
pos = 0,0
bindtime = -1
sprpriority = 3
ownpal = 1
removeongethit = 1
removetime = -1

[State -2,ex ]
type = Explod
trigger1 = anim = 100 && numexplod(100) = 1
anim = 7022
ID = 100
pos = 0,0
scale = 1.5,1.2
bindtime = -1
sprpriority = 3
ownpal = 1
removeongethit = 1
removetime = -1

[State -2,re ]
type = RemoveExplod
trigger1 = anim != 100
ID = 100


[State -2, Hit 1]
type = PlaySnd
triggerall = Time = 1
triggerall = stateno = 5000
triggerall = random = [0,600]
trigger1 = anim = 5000
trigger2 = anim = 5001
value = 5000,0

[State -2, Hit 1]
type = PlaySnd
triggerall = Time = 1
triggerall = stateno = 5000
triggerall = random = [0,600]
trigger1 = anim = 5002
trigger2 = anim = 5012
value = 5000,1

[State -2, Hit low]
type = PlaySnd
triggerall = Time = 1
triggerall = stateno = 5000
triggerall = random = [0,600]
trigger1 = anim = 5010
trigger2 = anim = 5011
value = 5000,2

[State -2, 1]
type = PlaySnd
triggerall = Time = 1
trigger1 = stateno = 5070
value = 5000,3

[State 610,re ]
type = RemoveExplod
trigger1 = anim = 47
ID = 600

[State 610,re ]
type = RemoveExplod
trigger1 = anim = 47
ID = 650

[State 610,re ]
type = RemoveExplod
trigger1 = anim = 47
ID = 651

;Auto Spirit Summon
[State -2,ss ]
type = Helper
triggerall = roundstate = 2
triggerall = movetype != H
trigger1 = timemod = 60,20 && numhelper(1050) < 5 && movetype != A && var(0) = 1
helpertype = normal
name = "Spirit"
ID = 1050
pos = 20,-130
stateno = 11050
ownpal = 1

;---------------------------------------------------------------------------
; States that are executed when in self's state file (use statedef -3)
;---------------------------------------------------------------------------

[Statedef -3]

[State -3,disp ]
type = DisplayToClipboard
trigger1 = var(0) >= 0
text = "AI = %d"
params = var(0)
;ignorehitpause = 0
;persistent = 1


[State -3, SetAI]
type = VarSet
triggerall = Alive && !IsHelper
triggerall = IsHomeTeam
trigger1 = TeamSide = 2
trigger2 = MatchNo > 1
var(58) = 1

[State -3, AI Helper - Compatibly Partnered Version]
type = Helper
trigger1 = !var(58) && !IsHelper
trigger1 = !NumHelper(9742)
trigger1 = Alive && RoundState = 2
trigger1 = NumPartner
trigger1 = Partner,SelfAnimExist(9741)
HelperType = normal
name = "AI Helper (Simul Version)"
ID = 9742
pos = 9999999,99999
StateNo = 9742
KeyCtrl = 1
PauseMoveTime = 2147483647
SuperMoveTime = 2147483647

[State -3, AI Helper]
type = Helper
triggerall = !var(58) && !IsHelper
triggerall = !NumHelper(9741)
triggerall = Alive && RoundState = 2
trigger1 = !NumPartner
trigger2 = !(Partner,SelfAnimExist(9741))
HelperType = normal
name = "AI Helper"
ID = 9741
pos = 9999999,99999
StateNo = 9741
KeyCtrl = 1
PauseMoveTime = 2147483647
SuperMoveTime = 2147483647

[State -3, TurnBackOnAI]
type = VarSet
trigger1 = var(58) = -2 && !IsHelper
trigger1 = Alive && RoundState = 2
var(58) = 1

[State -3, XOR]
type = VarSet
triggerall = var(58)!=1 && !IsHelper
triggerall = !var(56)
triggerall = var(57) = GameTime-1
trigger1 = command = "a" ^^ command = "a2"
trigger2 = command = "b" ^^ command = "b2"
trigger3 = command = "c" ^^ command = "c2"
trigger4 = command = "x" ^^ command = "x2"
trigger5 = command = "y" ^^ command = "y2"
trigger6 = command = "z" ^^ command = "z2"
trigger7 = command = "start" ^^ command = "start2"
trigger8 = command = "holda" ^^ command = "holda2"
trigger9 = command = "holdb" ^^ command = "holdb2"
trigger10 = command = "holdc" ^^ command = "holdc2"
trigger11 = command = "holdx" ^^ command = "holdx2"
trigger12 = command = "holdy" ^^ command = "holdy2"
trigger13 = command = "holdz" ^^ command = "holdz2"
trigger14 = command = "holdstart" ^^ command = "holdstart2"
trigger15 = command = "holdfwd" ^^ command = "holdfwd2"
trigger16 = command = "holdback" ^^ command = "holdback2"
trigger17 = command = "holdup" ^^ command = "holdup2"
trigger18 = command = "holddown" ^^ command = "holddown2"
trigger19 = command = "recovery" ^^ command = "recovery2"
var(58) = 1

[State -3, fiXOR]
type = VarAdd
trigger1 = var(56) > 0 && !IsHelper
var(56) = -1

[State -3, fuXOR]
type = VarSet
trigger1 = var(57) < GameTime-1 && !IsHelper
var(56) = 31
IgnoreHitPause = 1

[State -3, GameTimeVar]
type = VarSet
trigger1 = !IsHelper
var(57) = GameTime
IgnoreHitPause = 1

[State -3,atm ]
type = AttackMulSet
trigger1 = 1
value = (1+(.05*numhelper(1050)))*ifelse(var(0) = 1, 1, 0.75)
;ignorehitpause = 0
;persistent = 1


[State -3,va ]
type = VarSet
trigger1 = (numhelper(1050) > 0) && (Stateno != [1060,1090]) && (Stateno != [3000,3010])
v =  3
value = numhelper(1050)





;-------------------
;MEE Template Files
;   provided by
;  Kitsune Sniper
;       ***
; Thanks, Kitsune!
;-------------------
