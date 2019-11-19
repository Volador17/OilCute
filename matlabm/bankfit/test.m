
clear;
% load corn.mat
% x = m5spec.data;
% x = x';
% pr = propvals.data;
% pr = pr';
load CX.mat
load CY.mat
load VY.mat
load VX.mat
x = CX(:,:);
pr = CY(:,1);
pr = pr';



x = sgdiff(x,21,1);
x = smooth(x,5);
x = [x(132:313,:);x(469:676,:)];
[x,mx,sd] = normpathlength(x);

wind = 11;
num = 5;
%[result,allminSQ,allTQ]=mwcorrcv(x,pr,wind,num);

vx = VX;
vy = VY;



vx = sgdiff(vx,21,2);
vx = smooth(vx,5);
vx = [vx(132:313,:);vx(469:676,:)];
vx = normpathlength(vx);

wave1 = 1;
wave2 = 780;
[allTQ,allSQ,allRation,allIdx,allFit] = FitCroosValidation(x,wind,1:390);
%[allTQv,allSQv,allRationv,allIdxv,allFitv] = FitValidation(x,vx,wind,wave1,wave2);



%[resultv,allminSQv,allTQv]=mwcorrv(x,pr,vx,vy,wind,num);