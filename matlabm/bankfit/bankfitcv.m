function [result,result2,minSQ,TQ]=bankfitcv(x,pr,wind,wave1,wave2)
% 库光谱拟合法交互验证
% 输入：x，库光谱，pr，库光谱性质，wind，判断拟合是否充分进行移动相关系数和识别参数计算的窗口宽度，%
%wave1,判断拟合是否充分进行移动相关系数和识别参数计算的光谱区间初始位置，wave2，判断拟合是否充分进行移动相关系数和识别参数计算的光谱区间终了位置，
% 输出：result，性质显示，minSQ，最小移动相关系数,TQ，识别参数，
% result：（300+1）*性质：第1行为待识别样品的性质，第2行至非零数字行，为识别出来的库样品的性质，其他依次类推
% result2：300*库样品数目：每一列从第一行到非零数字行，为参与拟合的库样品的比例
% minSQ，1*校正集样品数目
% TQ，1*校正集样品数目
[m,n]=size(x);
for ii=1:n
    y=x(:,ii);
    xx=x;    
	xx(:,ii)=[];
    opr=pr;   
	opr(:,ii)=[];
	[ratio,ad]=bankfit(xx,y);
	% 判断是否拟合充分
	nx=xx(:,ad);
	yfit=nx*ratio';
	[T,SQ]=mwcorr(yfit(wave1:wave2,:),y(wave1:wave2,:),wind);
	TQ(ii)=T;
	minSQ(ii)=min(SQ);
	result((ii-1)*300+1,:)=pr(:,ii)';
	result2(1,ii)=ratio(1);
	[m1,n2]=size(ad);
	for j=1:n2
		result((ii-1)*300+j+1,:)=opr(:,ad(j))';
		result2(j,ii)=ratio(j);
	end
end