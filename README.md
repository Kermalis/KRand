# KRand

I wrote this .NET 8.0 library since nobody else seems to want inclusive max values in their randomizers for some reason.
Why do you guys have to be so weird?

Anyway I'm only making it a library since I've been using a version of this in pretty much everything I do, it has no dependencies, and it's lightning fast, so I might as well make it public.

It's like, really good.
It's based on the `Xoshiro256**` algorithm, which is the go-to in this time period I think.
You can supply your own seeds/states or not, the outputted values are evenly distributed, and it returns Boolean/8bit/16bit/32bit/64bit/Single/Double.
(Half is not supported because the resolution is too low for even distribution - I tried it and it skewed towards one side.
If you need Half support, you should try casting Single to it, but I don't know if this would solve the resolution issue...)
AND MOST IMPORTANTLY, THE MAXIMUM VALUES OF EACH RANGE ARE INCLUSIVE.

```cs
sbyte val = _rand.NextSByte(); // [sbyte.MinValue, sbyte.MaxValue]
short val = _rand.NextUInt16(-14, 23); // [-14, 23]
uint val = _rand.NextUInt32(); // [uint.MinValue, uint.MaxValue]
long val = _rand.NextUInt64(-5_000_000_000, 5_000_000_000); // [-5,000,000,000, 5,000,000,000]
float val = _rand.NextSingle(); // [0, 1]
double val = _rand.NextDouble(-500.123, 500.47); // [-500.123, 500.47]
bool val = _rand.NextBoolean(); // 1/2 (50%) True
bool val = _rand.NextBoolean(3, 10); // 3/10 (30%) True
bool val = _rand.NextBoolean(8, 447); // 8/447 (~1.78970917225951%) True
```

You can inherit from the `KRand` class too and add other stuff.
It has `Vector3` which just grabs 3 random floats, since it's very common to need 3 (color, position, etc.), so you can just add so much randomization like that but I kept Vector3 for everyone.

If you want to really see how evenly distributed it is, run the included tests.
I spent a lot of time and research to make the tests actually accurate with their reporting.

...Okay fine, of course I'll show you:

----
### Boolean: 50/50 chance | 1,000,000,000 iterations
```
False | 499,988,064 (50%)
 True | 500,011,936 (50%)
```
### Boolean: 30/70 chance | 1,000,000,000 iterations
```
False | 150,004,312 (30%)
 True | 349,995,688 (70%)
```
### Double: 50,000,000 iterations
```
Bucket 0 | [0.0, 0.1) | R AVG = 0.05000 ||| 5,002,518 (10.01%) | AVG = 0.05000
Bucket 1 | [0.1, 0.2) | R AVG = 0.15000 ||| 4,999,991 (10.00%) | AVG = 0.15000
Bucket 2 | [0.2, 0.3) | R AVG = 0.25000 ||| 5,000,154 (10.00%) | AVG = 0.25001
Bucket 3 | [0.3, 0.4) | R AVG = 0.35000 ||| 4,998,428 (10.00%) | AVG = 0.34999
Bucket 4 | [0.4, 0.5) | R AVG = 0.45000 ||| 4,999,807 (10.00%) | AVG = 0.45001
Bucket 5 | [0.5, 0.6) | R AVG = 0.55000 ||| 4,998,929 (10.00%) | AVG = 0.54999
Bucket 6 | [0.6, 0.7) | R AVG = 0.65000 ||| 5,000,104 (10.00%) | AVG = 0.64998
Bucket 7 | [0.7, 0.8) | R AVG = 0.75000 ||| 4,997,370 ( 9.99%) | AVG = 0.74999
Bucket 8 | [0.8, 0.9) | R AVG = 0.85000 ||| 5,000,855 (10.00%) | AVG = 0.85002
Bucket 9 | [0.9, 1.0] | R AVG = 0.95000 ||| 5,001,844 (10.00%) | AVG = 0.95001
```
### SByte: [-128, 127] range | 100,000,000 iterations
```
Bucket  0 | [-128.0000, -112.0625) | R AVG = -120.03125 ||| 6,247,921 (6.25%) | SUM = -752,877,326 | AVG = -120.50046
Bucket  1 | [-112.0625, - 96.1250) | R AVG = -104.09375 ||| 6,249,791 (6.25%) | SUM = -653,101,180 | AVG = -104.49968
Bucket  2 | [- 96.1250, - 80.1875) | R AVG = - 88.15625 ||| 6,249,963 (6.25%) | SUM = -553,146,671 | AVG = - 88.50399
Bucket  3 | [- 80.1875, - 64.2500) | R AVG = - 72.21875 ||| 6,251,926 (6.25%) | SUM = -453,265,928 | AVG = - 72.50021
Bucket  4 | [- 64.2500, - 48.3125) | R AVG = - 56.28125 ||| 6,251,060 (6.25%) | SUM = -353,194,202 | AVG = - 56.50149
Bucket  5 | [- 48.3125, - 32.3750) | R AVG = - 40.34375 ||| 6,249,829 (6.25%) | SUM = -253,123,610 | AVG = - 40.50089
Bucket  6 | [- 32.3750, - 16.4375) | R AVG = - 24.40625 ||| 6,251,392 (6.25%) | SUM = -153,166,308 | AVG = - 24.50115
Bucket  7 | [- 16.4375, -  0.5000) | R AVG = -  8.46875 ||| 6,248,948 (6.25%) | SUM = - 53,109,748 | AVG = -  8.49899
Bucket  8 | [-  0.5000,   15.4375) | R AVG =    7.46875 ||| 6,252,128 (6.25%) | SUM =   46,901,862 | AVG =    7.50174
Bucket  9 | [  15.4375,   31.3750) | R AVG =   23.40625 ||| 6,249,748 (6.25%) | SUM =  146,873,429 | AVG =   23.50070
Bucket 10 | [  31.3750,   47.3125) | R AVG =   39.34375 ||| 6,249,075 (6.25%) | SUM =  246,840,056 | AVG =   39.50025
Bucket 11 | [  47.3125,   63.2500) | R AVG =   55.28125 ||| 6,249,723 (6.25%) | SUM =  346,845,398 | AVG =   55.49772
Bucket 12 | [  63.2500,   79.1875) | R AVG =   71.21875 ||| 6,248,698 (6.25%) | SUM =  446,780,166 | AVG =   71.49972
Bucket 13 | [  79.1875,   95.1250) | R AVG =   87.15625 ||| 6,249,838 (6.25%) | SUM =  546,851,142 | AVG =   87.49845
Bucket 14 | [  95.1250,  111.0625) | R AVG =  103.09375 ||| 6,250,293 (6.25%) | SUM =  646,908,657 | AVG =  103.50053
Bucket 15 | [ 111.0625,  127.0000] | R AVG =  119.03125 ||| 6,249,667 (6.25%) | SUM =  746,843,599 | AVG =  119.50134
```
### UInt64: [1,000,000,000, 18,446,244,013,709,451,615] range | 50,000,000 iterations
```
Bucket 0 | [             1,000,000,000.0,  1,844,624,402,270,945,161.5) | R AVG =    922,312,201,635,472,580.75 ||| 4,994,986 ( 9.99%) | AVG =    922,474,043,596,388,696.65119
Bucket 1 | [ 1,844,624,402,270,945,161.5,  3,689,248,803,541,890,323.0) | R AVG =  2,766,936,602,906,417,742.25 ||| 4,998,958 (10.00%) | AVG =  2,766,910,383,201,381,190.82857
Bucket 2 | [ 3,689,248,803,541,890,323.0,  5,533,873,204,812,835,484.5) | R AVG =  4,611,561,004,177,362,903.75 ||| 4,999,225 (10.00%) | AVG =  4,611,519,291,355,730,455.11597
Bucket 3 | [ 5,533,873,204,812,835,484.5,  7,378,497,606,083,780,646.0) | R AVG =  6,456,185,405,448,308,065.25 ||| 5,001,415 (10.00%) | AVG =  6,456,205,489,846,172,361.83540
Bucket 4 | [ 7,378,497,606,083,780,646.0,  9,223,122,007,354,725,807.5) | R AVG =  8,300,809,806,719,253,226.75 ||| 4,999,282 (10.00%) | AVG =  8,300,866,809,324,480,645.48271
Bucket 5 | [ 9,223,122,007,354,725,807.5, 11,067,746,408,625,670,969.0) | R AVG = 10,145,434,207,990,198,388.25 ||| 5,002,822 (10.01%) | AVG = 10,145,444,838,329,452,382.21921
Bucket 6 | [11,067,746,408,625,670,969.0, 12,912,370,809,896,616,130.5) | R AVG = 11,990,058,609,261,143,549.75 ||| 5,002,139 (10.00%) | AVG = 11,990,031,099,442,315,059.18939
Bucket 7 | [12,912,370,809,896,616,130.5, 14,756,995,211,167,561,292.0) | R AVG = 13,834,683,010,532,088,711.25 ||| 5,001,811 (10.00%) | AVG = 13,834,757,620,438,795,154.50919
Bucket 8 | [14,756,995,211,167,561,292.0, 16,601,619,612,438,506,453.5) | R AVG = 15,679,307,411,803,033,872.75 ||| 4,999,848 (10.00%) | AVG = 15,679,071,349,122,356,922.71914
Bucket 9 | [16,601,619,612,438,506,453.5, 18,446,244,013,709,451,615.0] | R AVG = 17,523,931,813,073,979,034.25 ||| 4,999,514 (10.00%) | AVG = 17,524,134,996,440,728,893.58987
```

----
## KRandTesting Uses:
* [xUnit.net](https://github.com/xunit/xunit)