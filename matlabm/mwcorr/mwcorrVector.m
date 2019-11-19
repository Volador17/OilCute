function [TQ,SQ] = mwcorrVector(x,y,wind,topk)
% 移动相关系数法，确定移动相关系统的平均值阈值
% 输入：x向量，重复性光谱1，y向量，重复性光谱2，wind，窗口宽度，
% 输出：TQ向量，移动相关系统的平均值阈值，SQ，单个的移动相关系数阈值。

	[m,n]=size(x);% m，光谱变量数目，
	SQ=zeros(1,m);
	for k=1:m-wind
		xx=x(k:k+wind,:)';
		yy=y(k:k+wind,:)';
		SQ(k)=corre(xx,yy);
	end
	or=ones(1,wind)*corre(xx,yy);
	SQ(m-wind+1:end)=or;
	SQ=abs(SQ);
    [m1,n1]=sort(SQ);
    SQ(n1(1:topk-1)) = [];
    %SQ = SQ(n1(topk:end));
    
	oo=SQ(1:m-wind-topk+1);
	TQ = sum(oo)/(m-wind-topk+1);% 计算阈值
	%TQ = sum(oo.*oo)/(m-wind);% 计算阈值
end
