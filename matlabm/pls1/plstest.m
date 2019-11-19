clear;
load CX.mat
load CY.mat
load VX.mat
load VY.mat

cx = sgdiff(CX,21,2);
cx = smooth(cx,5);
cx = [cx(132:313,:);cx(469:676,:)];
%cx = normpathlength(cx);

cy = CY(:,1);

vx = sgdiff(VX,21,2);
vx = smooth(vx,5);
vx = [vx(132:313,:);vx(469:676,:)];
%vx = normpathlength(vx);
vy = VY(:,1);

%[EstimationY,MahDist,PRESS,SECV,cvR,yecv,SpecRes,Scores,Loads,calXres,valX,centerCompValue] = FullCrossValidate(cx,cy',6);
%train 
[Loads,Weights,b,Score_length,centerCompValue,centerSpecData,Ylast,XScores,SR,MD,X,ye,sec,nndt,mdt]=plsc(cx,cy',6);
[Scores,Loads, Weights,b,Score_length, centerSpecData , centerCompValue , mdt,nndt, sec1 ] = PLS1Train(cx,cy,6);
%[Ylast,SR,MD,ye,vR,sep,nd]=plsv(vx,vy',centerSpecData,centerCompValue,Loads,Weights,b,Score_length,XScores)

%plsp(centerSpecData,vx(:,1),Loads,Weights,b,XScores,Score_length,centerCompValue);
%Factor = 6;
%[Scores,Loads, Weights,b,Score_length, centerSpecData , centerCompValue, mdt,nndt ] = PLS1Train(cx,cy,Factor);
%[Ylast,SR,MD,nd] = PLS1Predictor(vx,Scores, Loads, Weights, b,  Score_length, centerSpecData, centerCompValue );
%[Ylast, SR, MD, nd] = PLS1CrossValidation(cx,cy,Factor);
%[Loads,Weights,b,Score_length,centerCompValue,centerSpecData,Ylast,XScores,SR,MD,X,ye,sec,nndt,mdt]=plsc(cx,cy',Factor);