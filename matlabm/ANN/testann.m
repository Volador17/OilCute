clear;
load CX.mat;
load CY.mat;
load VX.mat;
load VY.mat;

cx = sgdiff(CX,21,2);
cx = smooth(cx,5);
cx = [cx(132:313,:);cx(469:676,:)];
cy = CY(:,1);

vx = sgdiff(VX,21,2);
vx = smooth(vx,5);
vx = [vx(132:313,:);vx(469:676,:)];
vy = VY(:,1);

Factor = 9;

%先进行PLS分解
[Scores,Loads, Weights,b,Score_length, centerSpecData , centerCompValue , mdt,nndt, sec ] = PLS1Train(cx,cy,Factor,0);

%对Scores和y中心化
ms = mean(Scores);
mScores = Centring(Scores, ms); 
mCy = Centring(cy, centerCompValue);

%ann训练
trainf='trainlm';
hm=5;
f1='tansig';
f2='purelin';
tn=200;
traino=0.0001;
[w1,b1,w2,b2] = bann1(mScores,mCy,trainf,hm,f1,f2,tn,traino);

%交互验证
[Ylast,SR,MD,nd,XScores] = PLS1Predictor(cx,Scores, Loads, Weights, b,  Score_length, centerSpecData, centerCompValue,0 );
mScores = Centring(XScores, ms); 
yp = annp(mScores, w1,b1,w2,b2,f1,f2);
cyp = yp + centerCompValue;

%外部验证
[Ylast1,SR1,MD1,nd1,XScores1] = PLS1Predictor(vx,Scores, Loads, Weights, b,  Score_length, centerSpecData, centerCompValue,0 );
mScores = Centring(XScores1, ms); 
yp = annp(mScores, w1,b1,w2,b2,f1,f2);
vyp = yp + centerCompValue;






 