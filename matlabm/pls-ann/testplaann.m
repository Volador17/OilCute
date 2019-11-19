function testplaann()
%#function network
    load E:\3506\RIPP\matlabm\pls-ann\test.mat;
    Scores = Scores';
    eval('net = bann1(Scores,calY,trainf,hm,f1,f2,tn,traino);');
    save E:\3506\RIPP\matlabm\pls-ann\ann.mat net;
end