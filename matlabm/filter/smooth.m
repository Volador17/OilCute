%Savitzky-Golay 平滑程序;默认的多项式次数为3
%x:吸光度矩阵;
%m:平滑点数，必须大于5;
function xc=smooth(x,wp)
	k=3;
	xc=sgolayfilt(x,k,wp);
end
